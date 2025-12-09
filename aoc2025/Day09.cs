namespace aoc2025;

public class Day09 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        List<CornerTile> cornerTiles = new();
        foreach (string line in puzzleInput)
        {
            CornerTile cornerTile = new()
            {
                X = long.Parse(line.Split(',')[0]),
                Y = long.Parse(line.Split(',')[1])
            };
            cornerTiles.Add(cornerTile);
        }

        long largestAreaFound = 0;
        long maxX = long.MinValue;
        long maxY = long.MinValue;
        long minX = long.MaxValue;
        long minY = long.MaxValue;

        foreach (var cornerTile in cornerTiles)
        {
            foreach (var other in cornerTiles)
            {
                if (other == cornerTile)
                    continue;

                long area = (Math.Abs(cornerTile.X - other.X) + 1) * (Math.Abs(cornerTile.Y - other.Y) + 1);
                cornerTile.AreaWithOtherCorners[other] = area;

                if (area > largestAreaFound)
                    largestAreaFound = area;

                if (other.X > maxX)
                    maxX = other.X;
                if (other.Y > maxY)
                    maxY = other.Y;
                if (other.X < minX)
                    minX = other.X;
                if (other.Y < minY)
                    minY = other.Y;
            }
        }

        Dictionary<(long x, long y), char> map = new();

        char outsideMarker = '~';

        for (var index = 0; index < cornerTiles.Count; index++)
        {
            var cornerTile = cornerTiles[index];
            var nextIndex = (index + 1) % cornerTiles.Count;
            var nextTile = cornerTiles[nextIndex];

            if (cornerTile.X == nextTile.X)
            {
                var min = Math.Min(cornerTile.Y, nextTile.Y);
                var max = Math.Max(cornerTile.Y, nextTile.Y);

                for (long y = min; y <= max; y++)
                {
                    map[(cornerTile.X, y)] = '#';
                }
            }
            else if (cornerTile.Y == nextTile.Y)
            {
                var min = Math.Min(cornerTile.X, nextTile.X);
                var max = Math.Max(cornerTile.X, nextTile.X);

                for (long x = min; x <= max; x++)
                {
                    Console.WriteLine($"{x}, {cornerTile.Y}");
                    map[(x, cornerTile.Y)] = '#';
                }
            }

            map[(cornerTile.X, cornerTile.Y)] = 'O';
        }

        //Walk around the outer edge and mark as outside. Hope starting to the left and going up works.
        MarkOutside(cornerTiles, map, outsideMarker);


        long largestAreaFoundPart2 = 0;

        int progress = 0;
        foreach (var cornerTile in cornerTiles)
        {
            foreach (var (other, area) in cornerTile.AreaWithOtherCorners)
                //foreach (var other in cornerTiles)
            {
                if (other == cornerTile)
                    continue;

                //long area = (Math.Abs(cornerTile.X - other.X) + 1) * (Math.Abs(cornerTile.Y - other.Y) + 1);
                if (area <= largestAreaFoundPart2)
                    continue;

                if (IsAreaOutside(cornerTile, other, map, outsideMarker))
                    continue;

                largestAreaFoundPart2 = area;
            }

            progress++;
            Console.WriteLine($"Progress: {progress}/{cornerTiles.Count}");
        }

        return (largestAreaFound, largestAreaFoundPart2);
    }

    private static void MarkOutside(List<CornerTile> cornerTiles, Dictionary<(long x, long y), char> map, char outsideMarker)
    {
        var startX = cornerTiles[0].X - 1; //Start just left of first corner
        var startY = cornerTiles[0].Y;
        var currentX = startX;
        var currentY = startY;
        var directionX = 0;
        var directionY = -1; //Start going up
        var wallDirectionX = 1;
        var wallDirectionY = 0;

        do
        {
            //Walk into wall
            if (map.TryGetValue((currentX, currentY), out var value) && (value == '#' || value == 'O'))
            {
                //Backtrack
                currentX -= directionX;
                currentY -= directionY;

                var newDirectionX = -wallDirectionX;
                var newDirectionY = -wallDirectionY;

                wallDirectionX = directionX;
                wallDirectionY = directionY;

                directionX = newDirectionX;
                directionY = newDirectionY;

            }
            //Reach outside corner
            else if (!map.ContainsKey((currentX + wallDirectionX, currentY + wallDirectionY)))
            {
                map[(currentX, currentY)] = outsideMarker;
                directionX = wallDirectionX;
                directionY = wallDirectionY;
                currentX += directionX;
                currentY += directionY;

                //Find wall #
                if (map.TryGetValue((currentX-1, currentY), out var hit) && (hit == '#' || hit == 'O'))
                {
                    wallDirectionX = -1;
                    wallDirectionY = 0;
                }
                else if (map.TryGetValue((currentX, currentY-1), out hit) && (hit == '#' || hit == 'O'))
                {
                    wallDirectionX = 0;
                    wallDirectionY = -1;
                }
                else if (map.TryGetValue((currentX+1, currentY), out hit) && (hit == '#' || hit == 'O'))
                {
                    wallDirectionX = 1;
                    wallDirectionY = 0;
                }
                else if (map.TryGetValue((currentX, currentY+1), out hit) && (hit == '#' || hit == 'O'))
                {
                    wallDirectionX = 0;
                    wallDirectionY = 1;
                }
                else
                {
                    throw new Exception("Lost wall direction");
                }
            }

            map[(currentX, currentY)] = outsideMarker;

            //PrintMap(minY - 1, maxY + 1, minX - 1, maxX + 1, map);
            //Console.WriteLine($"Current position: {currentX}, {currentY}, direction: {directionX}, {directionY}, wall direction: {wallDirectionX}, {wallDirectionY}");
            //Console.ReadLine();

            currentX += directionX;
            currentY += directionY;
        } while (currentX != startX || currentY != startY);


        Console.WriteLine($"Finished marking outside area. Outside marker count: {map.Count(kvp => kvp.Value == outsideMarker)}");
    }

    private static bool IsAreaOutside(CornerTile cornerTile, CornerTile other, Dictionary<(long x, long y), char> map,
        char outsideMarker)
    {
        long areaMinX = Math.Min(cornerTile.X, other.X);
        long areaMaxX = Math.Max(cornerTile.X, other.X);
        long areaMinY = Math.Min(cornerTile.Y, other.Y);
        long areaMaxY = Math.Max(cornerTile.Y, other.Y);

        for (long y = areaMinY + 1; y < areaMaxY; y++)
        {
            if (map.TryGetValue((areaMinX + 1, y), out var val) && val=='#')
            {
                return true;
            }
            if (map.TryGetValue((areaMaxX - 1, y), out val) && val=='#')
            {
                return true;
            }
        }
        for (long x = areaMinX + 1; x < areaMaxX; x++)
        {
            if (map.TryGetValue((x, areaMinY + 1), out var val) && val=='#')
            {
                return true;
            }
            if (map.TryGetValue((x, areaMaxY - 1), out val) && val=='#')
            {
                return true;
            }
        }

        /*for (long y = areaMinY; y <= areaMaxY; y++)
        {
            if (map.TryGetValue((areaMinX, y), out var val) && val == outsideMarker)
            {
                return true;
            }
            if (map.TryGetValue((areaMaxX, y), out val) && val == outsideMarker)
            {
                return true;
            }
        }
        for (long x = areaMinX; x <= areaMaxX; x++)
        {
            if (map.TryGetValue((x, areaMinY), out var val) && val == outsideMarker)
            {
                return true;
            }
            if (map.TryGetValue((x, areaMaxY), out val) && val == outsideMarker)
            {
                return true;
            }
        }*/


        return false;
    }

    private static void PrintMap(long minY, long maxY, long minX, long maxX, Dictionary<(long x, long y), char> map)
    {
        for (long y = minY - 1; y <= maxY; y++)
        {
            for (long x = minX - 1; x <= maxX; x++)
            {
                if (!map.TryGetValue((x, y), out char c))
                    c = '.';
                Console.Write(c);
            }
            Console.WriteLine();
        }
    }

    private class CornerTile
    {
        public long X { get; init; }
        public long Y { get; init; }

        public Dictionary<CornerTile, long> AreaWithOtherCorners { get; set; } = new();
    }

    public string GetExampleInput()
    {
        return "7,1\n11,1\n11,7\n9,7\n9,5\n2,5\n2,3\n7,3";
    }
}