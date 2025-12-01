using Spectre.Console;

namespace aoc2025;

public class Day01
{
    public static async Task Run()
    {
        var input = await PasteInput.PasteInputPrompt();
        AnsiConsole.Clear();
        AnsiConsole.WriteLine("Day 01:");

        int position = 50;
        
        int part1ZeroCounter = 0;
        int part2ZeroCounter = 0;
        
        foreach (var line in input)
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
        
        Console.WriteLine("Part 1: " + part1ZeroCounter);
        Console.WriteLine("Part 2: " + part2ZeroCounter);
    }
}