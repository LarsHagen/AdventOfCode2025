using aoc2025;
using Spectre.Console;

AnsiConsole.Markup("AOC 2025");

int day = AnsiConsole.Prompt(new TextPrompt<int>("Please type day to run:"));

if (day == 1)
{
    await Day01.Run();
}