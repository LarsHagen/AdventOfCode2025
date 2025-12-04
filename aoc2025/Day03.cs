namespace aoc2025;

public class Day03 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        List<List<int>> banks = new();

        foreach (var line in puzzleInput)
        {
            banks.Add(line.Select(c => int.Parse(c.ToString())).ToList());
        }

        int part1 = 0;

        for (var i = 0; i < banks.Count; i++)
        {
            var bank = banks[i];
            var first = bank[0..^1].Max();
            var second = bank[(bank.IndexOf(first) + 1)..].Max();

            string combined = $"{first}{second}";
            
            part1 += int.Parse(combined);
        }
        
        long part2 = 0;
        
        for (var i = 0; i < banks.Count; i++)
        {
            var bank = banks[i];

            int indexOfLastDigit = -1;
            string combined = string.Empty;
            
            for (int j = 11; j >= 0; j--)
            {
                var digit = bank[(indexOfLastDigit + 1)..^j].Max();
                
                indexOfLastDigit += bank[(indexOfLastDigit + 1)..^j].IndexOf(digit) + 1;

                combined += digit;
            }
            
            part2 += long.Parse(combined);
        }

        return (part1, part2);
    }

    public string GetExampleInput()
    {
        return "987654321111111\n811111111111119\n234234234234278\n818181911112111";
    }
}