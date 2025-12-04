namespace aoc2025;

public interface IAocDay
{
    Task<(object part1, object part2)> Run(List<string> puzzleInput);
    string GetExampleInput();
}