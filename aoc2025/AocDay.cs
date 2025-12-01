namespace aoc2025;

public interface AocDay
{
    Task<(string part1, string part2)> Run(List<string> puzzleInput);
    string GetExampleInput();
}