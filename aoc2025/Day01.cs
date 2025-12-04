namespace aoc2025;

public class Day01 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        int position = 50;
        
        int part1ZeroCounter = 0;
        int part2ZeroCounter = 0;
        
        foreach (var line in puzzleInput)
        {
            if (string.IsNullOrEmpty(line))
                continue;
            
            int direction = line.StartsWith("L") ? -1 : 1;
            int count = int.Parse(line[1..]);

            for (int i = 0; i < count; i++)
            {
                position += direction;
                if (position < 0)
                    position += 100;
                if (position >= 100)
                    position -= 100;
                
                if (position == 0)
                    part2ZeroCounter++;
            }

            if (position == 0)
            {
                part1ZeroCounter++;
            }
        }
        return (part1ZeroCounter, part2ZeroCounter);
    }

    public string GetExampleInput() => "L68\nL30\nR48\nL5\nR60\nL55\nL1\nL99\nR14\nL82";
}