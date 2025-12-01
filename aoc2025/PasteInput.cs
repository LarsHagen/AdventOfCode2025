using Spectre.Console;

namespace aoc2025;

public class PasteInput
{
    public static List<string> PasteInputPrompt()
    {
        AnsiConsole.MarkupLine("[blue]Paste puzzle input[/] and press enter or [blue]leave blank to use example input[/]:");
        var lines = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            
            lines.Add(line);
        }
        
        return lines;
    }
}