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
        long shortestPressCount = long.MaxValue;

        PriorityQueue<(int nextButtonIndex, int pressCount, List<int> currentJoltageLevels), int> queue = new();

        var initialDistanceToTarget = machine.Joltage.Sum();

        for (int i = 0; i < machine.ButtonPressPatterns.Count; i++)
        {
            List<int> initialJoltage = new();
            for (int j = 0; j < machine.Joltage.Count; j++)
            {
                initialJoltage.Add(0);
            }
            queue.Enqueue((i, 0, initialJoltage), initialDistanceToTarget);
        }

        Dictionary<string, int> visitedStates = new();

        while (queue.Count > 0)
        {

            //Console.WriteLine(queue.Count);
            var (nextButtonIndex, pressCount, currentJoltage) = queue.Dequeue();

            bool overshot = false;
            for (var index = 0; index < currentJoltage.Count; index++)
            {
                var jultageLevel = currentJoltage[index];
                var targetJoltageLevel = machine.Joltage[index];
                if (jultageLevel > targetJoltageLevel)
                    overshot = true;
            }
            if (overshot)
                continue;

            string currentPatternKey = string.Join("", currentJoltage.Select(b => b.ToString()));

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

            foreach (var i in nextButton)
            {
                currentJoltage[i]++;
            }

            //Console.WriteLine("New pattern: " + string.Join("", currentPattern.Select(b => b ? '#' : '.')));




            pressCount++;

            if (currentJoltage.SequenceEqual(machine.Joltage))
            {
                Console.WriteLine("Pattern matched with press count: " + pressCount);
                if (pressCount < shortestPressCount)
                {
                    shortestPressCount = pressCount;
                }
            }
            else
            {
                var distanceToTarget = 0;
                for (var index = 0; index < currentJoltage.Count; index++)
                {
                    var distance = machine.Joltage[index] - currentJoltage[index];
                    distanceToTarget += distance;
                }

                Console.WriteLine($"{distanceToTarget}");

                for (int i = 0; i < machine.ButtonPressPatterns.Count; i++)
                {
                    List<int> joltageCopy = new(currentJoltage);
                    queue.Enqueue((i, pressCount, joltageCopy), distanceToTarget);
                }
            }

            //Console.ReadLine();



        }

        return shortestPressCount;
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