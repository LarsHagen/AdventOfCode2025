using Spectre.Console;

namespace aoc2025;

public class PasteInput
{
    public static async Task<List<string>> PasteInputPrompt()
    {
        AnsiConsole.Markup("Paste Input or sample and press Enter:");
        var lines = new List<string>();
        
        lines.Add(Console.ReadLine());
        var cts = new CancellationTokenSource();
        while (true)
        {
            var readTask = Task.Run(() => Console.ReadLine());
            var timeOutTask = Task.Delay(500, cts.Token);
            var completedTask = await Task.WhenAny(readTask, timeOutTask);

            if (completedTask == timeOutTask)
                break;
            
            cts.Cancel();
            cts = new CancellationTokenSource();
            lines.Add(readTask.Result);
        }

        return lines;
    }
}