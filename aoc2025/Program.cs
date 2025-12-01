using aoc2025;
using Spectre.Console;

AnsiConsole.Write(new Rule("[bold green]AOC 2025[/]"));

Dictionary<int, AocDay> implementedDays = new()
{
    { 1, new Day01() }
};

while (true)
{
    int day = AnsiConsole.Prompt(new TextPrompt<int>("Please type day to run, (Ctrl+C to exit):"));

    if (!implementedDays.TryGetValue(day, out AocDay aocDay))
    {
        AnsiConsole.MarkupLine($"[red]Day {day} not implemented.[/]");
        continue;
    }
    
    var input = await PasteInput.PasteInputPrompt();
    if (input.Count == 0)
    {
        AnsiConsole.MarkupLine($"[yellow]No input. Using example input.[/]");
        input = aocDay.GetExampleInput().Split(Environment.NewLine).ToList();
    }
    else
    {
        AnsiConsole.MarkupLine($"[yellow]Input captured.[/]");
    }

    AnsiConsole.Write(new Rule($"[green]Day {day}[/]"));

    try
    {
        var (part1, part2) = await aocDay.Run(input);
        AnsiConsole.MarkupLine($"[blue]Part 1: [/] {part1}");
        AnsiConsole.MarkupLine($"[blue]Part 2: [/] {part2}");
    }
    catch (Exception e)
    {
        AnsiConsole.WriteException(e);
    }
    
    AnsiConsole.Write(new Rule());
}