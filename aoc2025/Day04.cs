using System.Text;

namespace aoc2025;

public class Day04 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        int firstPass = DoRemovePass(ref puzzleInput);

        int totalRemoveable = firstPass;
        int removedThisPass = 0;
        do
        {
            removedThisPass = DoRemovePass(ref puzzleInput);
            totalRemoveable += removedThisPass;
        } while (removedThisPass > 0);
        
        return (firstPass, totalRemoveable);

    }
    
    private int DoRemovePass(ref List<string> map)
    {
        int accessable = 0;
        
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                var input = map[y][x];
                
                if (input == '.')
                    continue;

                int neighbors = 0;
                
                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        if (ky == 0 && kx == 0)
                            continue;
                        
                        var nX = x + kx;
                        var nY = y + ky;
                        
                        if (nX < 0 || nY < 0 || nX >= map[y].Length || nY >= map.Count)
                            continue;
                        
                        var neighbor = map[nY][nX];
                        if (neighbor != '.')
                            neighbors += 1;
                    }
                }
                
                if (neighbors < 4)
                {
                    StringBuilder sb = new (map[y]) { [x] = 'x' };
                    map[y] = sb.ToString();
                    accessable += 1;
                }
            }
        }
        
        for (int y = 0; y < map.Count; y++)
        {
            map[y] = map[y].Replace('x', '.');
        }

        return accessable;
    }

    public string GetExampleInput()
    {
        return
            "..@@.@@@@.\n@@@.@.@.@@\n@@@@@.@.@@\n@.@@@@..@.\n@@.@@@@.@@\n.@@@@@@@.@\n.@.@.@.@@@\n@.@@@.@@@@\n.@@@@@@@@.\n@.@.@@@.@.";
    }
}