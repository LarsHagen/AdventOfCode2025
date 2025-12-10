using Google.OrTools.LinearSolver;

namespace aoc2025;

public class Day10 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        List<Machine> machines = new();
        foreach (var line in puzzleInput)
        {
            var parts = line.Split(' ');

            List<bool> lightPatternToMatch = new();
            List<List<int>> buttonPressPatterns = new();
            List<int> joltage = new();

            foreach (var part in parts)
            {
                if (part.Contains('['))
                {
                    var lightPatternString = part.Replace("[","").Replace("]","");
                    lightPatternToMatch = lightPatternString.Select(ch => ch == '#').ToList();
                    continue;
                }

                if (part.Contains('('))
                {
                    var buttonString = part.Replace("(","").Replace(")","");
                    List<int> buttonIndexes = buttonString.Split(',').Select(s => int.Parse(s)).ToList();
                    buttonPressPatterns.Add(buttonIndexes);
                    continue;
                }

                if (part.Contains('{'))
                {
                    var joltageString = part.Replace("{","").Replace("}","");
                    joltage = joltageString.Split(',').Select(s => int.Parse(s)).ToList();
                    continue;
                }
            }

            Machine machine = new()
            {
                LightPatternToMatch = lightPatternToMatch,
                ButtonPressPatterns = buttonPressPatterns,
                Joltage = joltage,
                Lights = new List<bool>(new bool[lightPatternToMatch.Count])
            };

            machines.Add(machine);
        }

        long part1 = 0;
        long part2 = 0;

        foreach (var machine in machines)
        {
            //Print machine info
            Console.WriteLine("Machine:");
            Console.WriteLine("Light pattern to match: " + string.Join("", machine.LightPatternToMatch.Select(b => b ? '#' : '.')));
            Console.WriteLine("Button press patterns:");
            for (int i = 0; i < machine.ButtonPressPatterns.Count; i++)
            {
                Console.WriteLine($" Button {i}: " + string.Join(",", machine.ButtonPressPatterns[i]));
            }
            Console.WriteLine("Joltage: " + string.Join(",", machine.Joltage));


            long leastPresses = GetLeastAmountOfPressesToMatchPattern(machine);
            long leastJoltagePresses = GetLeastAmountOfPressesToJoltageLevel(machine);
            Console.WriteLine(leastPresses);
            Console.WriteLine(leastJoltagePresses);

            //Console.ReadLine();

            part1 += leastPresses;
            part2 += leastJoltagePresses;
        }

        return (part1, part2);
    }

    private long GetLeastAmountOfPressesToMatchPattern(Machine machine)
    {
        long shortestPressCount = long.MaxValue;

        Queue<(int nextButtonIndex, int pressCount, List<bool> currentPattern)> queue = new();
        for (int i = 0; i < machine.ButtonPressPatterns.Count; i++)
        {
            queue.Enqueue((i, 0, new List<bool>(machine.Lights)));
        }

        Dictionary<string, int> visitedStates = new();

        while (queue.Count > 0)
        {
            //Console.WriteLine(queue.Count);
            var (nextButtonIndex, pressCount, currentPattern) = queue.Dequeue();

            string currentPatternKey = string.Join("", currentPattern.Select(b => b ? '1' : '0'));

            if (visitedStates.TryGetValue(currentPatternKey, out var count))
            {
                if (count < pressCount)
                    continue;
            }
            visitedStates[currentPatternKey] = pressCount;


            if (pressCount >= shortestPressCount - 1)
                continue;

            var nextButton = machine.ButtonPressPatterns[nextButtonIndex];

            //Console.WriteLine("Current pattern: " + string.Join("", currentPattern.Select(b => b ? '#' : '.')));
            //Console.WriteLine("Pressing button " + nextButtonIndex + " which toggles lights at indexes: " + string.Join(",", nextButton));

            for (int i = 0; i < currentPattern.Count; i++)
            {
                if (nextButton.Contains(i))
                {
                    currentPattern[i] = !currentPattern[i];
                }
            }

            //Console.WriteLine("New pattern: " + string.Join("", currentPattern.Select(b => b ? '#' : '.')));



            pressCount++;

            if (currentPattern.SequenceEqual(machine.LightPatternToMatch))
            {
                Console.WriteLine("Pattern matched with press count: " + pressCount);
                if (pressCount < shortestPressCount)
                {
                    shortestPressCount = pressCount;
                }
            }
            else
            {


                for (int i = 0; i < machine.ButtonPressPatterns.Count; i++)
                {
                    if (i == nextButtonIndex)
                        continue;
                    List<bool> patternCopy = new(currentPattern);
                    queue.Enqueue((i, pressCount, patternCopy));
                }
            }

            //Console.ReadLine();
        }

        return shortestPressCount;
    }

    private long GetLeastAmountOfPressesToJoltageLevel(Machine machine)
    {
        Solver solver = Solver.CreateSolver("SCIP");

        // Create variables for how many times each button is pressed
        Variable[] buttonPresses = new Variable[machine.ButtonPressPatterns.Count];
        for (int i = 0; i < machine.ButtonPressPatterns.Count; i++)
        {
            // Each button can be pressed 0 or more times (up to a reasonable maximum)
            buttonPresses[i] = solver.MakeIntVar(0.0, 1000.0, $"button_{i}");
        }

        // For each joltage position, create a constraint
        for (int pos = 0; pos < machine.Joltage.Count; pos++)
        {
            //Create a constraint that must be the exact joltage
            Constraint constraint = solver.MakeConstraint(machine.Joltage[pos], machine.Joltage[pos], $"joltage_pos_{pos}");

            //Find all buttons that affect this position and make them add 1 to the constraint
            for (int btnIdx = 0; btnIdx < machine.ButtonPressPatterns.Count; btnIdx++)
            {
                if (machine.ButtonPressPatterns[btnIdx].Contains(pos))
                {
                    constraint.SetCoefficient(buttonPresses[btnIdx], 1);
                }
            }
        }

        // Setup objective. Make each variable contribute to the objective
        Objective objective = solver.Objective();
        for (int i = 0; i < buttonPresses.Length; i++)
        {
            objective.SetCoefficient(buttonPresses[i], 1);
        }
        objective.SetMinimization(); //Find the least amount of button presses

        // Solve
        Solver.ResultStatus resultStatus = solver.Solve();

        if (resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            //Get value of each variable used by the solver and sum them up
            long total = 0;
            for (int i = 0; i < machine.ButtonPressPatterns.Count; i++)
            {
                long presses = (long)Math.Round(buttonPresses[i].SolutionValue());
                total += presses;
            }
            return total;
        }

        throw new Exception("No solution found for joltage matching");
    }

    private class Machine
    {
        public List<bool> LightPatternToMatch = new();
        public List<bool> Lights = new();
        public List<List<int>> ButtonPressPatterns = new();
        public List<int> Joltage = new();
    }

    public string GetExampleInput()
    {
        return
            "[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}\n[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}\n[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}";
    }
}