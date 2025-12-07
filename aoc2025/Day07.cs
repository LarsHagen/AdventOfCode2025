namespace aoc2025;

public class Day07 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        var map = ConvertToCharArray(puzzleInput);

        int splitCount = 0;

        for (int y = 1; y < puzzleInput.Count; y++)
        {
            for (int x = 0; x < puzzleInput[y].Length; x++)
            {
                var above = map[y - 1][x];
                var current = map[y][x];

                if (current == '|')
                    continue;

                if (current == '^' && (above == '|' || above == 'S'))
                {
                    map[y][x - 1] = '|';
                    map[y][x + 1] = '|';
                    splitCount++;
                    continue;
                }

                if (above == 'S' || above == '|')
                    map[y][x] = '|';
            }
        }

        /*for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                Console.Write(map[y][x]);
            }
            Console.WriteLine();
        }*/

        Dictionary<(int x, int y), long> cachedTimelinesFromSplitterPositions = new();

        for (int y = map.Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                var current = map[y][x];
                if (current == '^')
                {
                    var splitterBelowLeft = GetSplitterBelow(x - 1, y, map);
                    var splitterBelowRight = GetSplitterBelow(x + 1, y, map);

                    long options = 0;
                    if (splitterBelowLeft != (-1, -1))
                    {
                        if (cachedTimelinesFromSplitterPositions.TryGetValue(splitterBelowLeft, out long cachedOptionsLeft))
                        {
                            options += cachedOptionsLeft;
                        }
                        else
                        {
                            throw new Exception("Missing cached options for splitter below left at " + splitterBelowLeft);
                        }
                    }
                    else
                    {
                        options++;
                    }
                    if (splitterBelowRight != (-1, -1))
                    {
                        if (cachedTimelinesFromSplitterPositions.TryGetValue(splitterBelowRight, out long cachedOptionsRight))
                        {
                            options += cachedOptionsRight;
                        }
                        else
                        {
                            throw new Exception("Missing cached options for splitter below right at " + splitterBelowRight);
                        }
                    }
                    else
                    {
                        options++;
                    }

                    cachedTimelinesFromSplitterPositions[(x, y)] = options;
                }
            }
        }

        var mostOptions = cachedTimelinesFromSplitterPositions.Values.Max();

        return (splitCount, mostOptions);
    }

    private (int x, int y) GetSplitterBelow(int x, int startY, char[][] map)
    {
        for (int y = startY; y < map.Length; y++)
        {
            var current = map[y][x];
            if (current == '^')
            {
                return (x, y);
            }
        }

        return (-1, -1);
    }

    private char[][] ConvertToCharArray(List<string> puzzleInput)
    {
        char[][] map = new char[puzzleInput.Count][];
        for (int i = 0; i < puzzleInput.Count; i++)
        {
            map[i] = puzzleInput[i].ToCharArray();
        }
        return map;
    }

    public string GetExampleInput()
    {
        return
            ".......S.......\n...............\n.......^.......\n...............\n......^.^......\n...............\n.....^.^.^.....\n...............\n....^.^...^....\n...............\n...^.^...^.^...\n...............\n..^...^.....^..\n...............\n.^.^.^.^.^...^.\n...............";
    }
}