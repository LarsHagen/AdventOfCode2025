using Spectre.Console;

namespace aoc2025;

public class PasteInput
{
    public static async Task<List<string>> PasteInputPrompt()
    {
        const bool printInput = false;

        AnsiConsole.MarkupLine("[blue]Paste puzzle input[/] and press enter or [blue]leave blank to use example input[/]:");

        var lines = new List<string>();
        var currentLine = string.Empty;
        var inputStarted = false;

        while (true)
        {
            // Check if there's input available without blocking
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);
                inputStarted = true;

                if (key.Key == ConsoleKey.Enter)
                {
                    lines.Add(currentLine);
                    currentLine = string.Empty;
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    currentLine += key.KeyChar;
                }
            }
            else
            {
                // No input available - check if we should exit
                if (inputStarted)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                    }
                    break;
                }

                // Small delay to avoid busy-waiting
                await Task.Delay(10);
            }
        }

        if (printInput)
        {
            AnsiConsole.MarkupLine("[green]Captured Input:[/]");
            foreach (var line in lines)
            {
                AnsiConsole.MarkupLine(line);
            }
        }

        if (lines.Count == 1 && string.IsNullOrWhiteSpace(lines[0]))
        {
            lines.Clear();
        }

        return lines;
    }
}