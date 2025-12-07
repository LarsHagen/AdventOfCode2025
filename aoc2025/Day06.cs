namespace aoc2025;

public class Day06 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        var mathProblemsPart1 = Part1Parser(puzzleInput);
        var mathProblemsPart2 = Part2Parser(puzzleInput);
        return (GetResult(mathProblemsPart1), GetResult(mathProblemsPart2));
    }

    private List<MathProblem> Part2Parser(List<string> puzzleInput)
    {
        List<MathProblem> mathProblems = new();

        int rows = puzzleInput.Count;
        int cols = puzzleInput.Max(line => line.Length);

        for (int y = 0; y < rows; y++)
        {
            puzzleInput[y] = puzzleInput[y].PadRight(cols, ' ');
            puzzleInput[y] = ' ' + puzzleInput[y];
        }

        MathProblem currentMathProblem = new();

        for (int x = cols; x >= 0; x--)
        {
            string numberAsString = "";
            for (int y = 0; y < rows - 1; y++)
            {
                char character = puzzleInput[y][x];
                numberAsString += character;
            }
            bool isEmpty = string.IsNullOrWhiteSpace(numberAsString);

            if (isEmpty)
            {
                var operationChar = puzzleInput[rows-1][x+1];
                if (operationChar == '+')
                    currentMathProblem.Operation = new Add();
                else if (operationChar == '*')
                    currentMathProblem.Operation = new Multiply();
                else
                    throw new Exception("Unknown operation character: " + operationChar);

                mathProblems.Add(currentMathProblem);
                currentMathProblem = new MathProblem();
            }
            else
            {
                currentMathProblem.Operands.Add(long.Parse(numberAsString));
            }
        }

        return mathProblems;
    }


    private List<MathProblem> Part1Parser(List<string> puzzleInput)
    {
        List<MathProblem> mathProblems = new();

        for (int y = 0; y < puzzleInput.Count; y++)
        {
            var splitOnEmpty = puzzleInput[y].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (mathProblems.Count == 0)
            {
                for (int x = 0; x < splitOnEmpty.Length; x++)
                    mathProblems.Add(new MathProblem());
            }

            for (int x = 0; x < splitOnEmpty.Length; x++)
            {
                MathProblem mathProblem = mathProblems[x];

                if (long.TryParse(splitOnEmpty[x], out long operand))
                {
                    mathProblem.Operands.Add(operand);
                }
                else if (splitOnEmpty[x].Contains("+"))
                {
                    mathProblem.Operation = new Add();
                }
                else if (splitOnEmpty[x].Contains("*"))
                {
                    mathProblem.Operation = new Multiply();
                }
                else
                {
                    throw new Exception("Unknown token in math problem: " + splitOnEmpty[x]);
                }
            }
        }

        return mathProblems;
    }

    private long GetResult(List<MathProblem> mathProblems)
    {
        List<long> results = new();
        foreach (var mathProblem in mathProblems)
        {
            long result = mathProblem.Operands[0];
            for (int i = 1; i < mathProblem.Operands.Count; i++)
            {
                result = mathProblem.Operation.Perform(result, mathProblem.Operands[i]);
            }
            results.Add(result);

            Console.WriteLine($"{mathProblem}, result: {result}");
        }
        return results.Sum();
    }

    public string GetExampleInput()
    {
        return "123 328  51 64 \n 45 64  387 23 \n  6 98  215 314\n*   +   *   +  ";
    }

    private class MathProblem
    {
        public Operation Operation { get; set; }
        public List<long> Operands { get; set; } = new();

        public override string ToString()
        {
            return $"Operands: {string.Join(", ", Operands)} Operation: {Operation.GetType().Name}";
        }
    }

    private abstract class Operation
    {
        public abstract long Perform(long a, long b);
    }

    private class Add : Operation
    {
        public override long Perform(long a, long b)
        {
            return a + b;
        }
    }

    private class Multiply : Operation
    {
        public override long Perform(long a, long b)
        {
            return a * b;
        }
    }
}