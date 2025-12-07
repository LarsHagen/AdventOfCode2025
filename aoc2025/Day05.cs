namespace aoc2025;

public class Day05 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        var ranges = new List<(long start, long end)>();
        var ingredients = new List<long>();

        foreach (var line in puzzleInput)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.Contains('-'))
            {
                var start = long.Parse(line.Split('-')[0]);
                var end = long.Parse(line.Split('-')[1]);
                ranges.Add((start, end));
            }
            else
            {
                ingredients.Add(long.Parse(line));
            }
        }

        long freshCount = 0;

        foreach (var ingredient in ingredients)
        {
            bool inAnyRange = false;
            foreach (var range in ranges)
            {
                if (range.start <= ingredient && ingredient <= range.end)
                {
                    inAnyRange = true;
                    break;
                }
            }

            if (inAnyRange)
            {
                freshCount++;
            }
        }

        var rangesNoOverlap = new List<(long start, long end)>();
        ranges = ranges.OrderBy(r => r.start).ToList();
        foreach (var range in ranges)
        {
            var start = range.start;
            var end = range.end;

            bool skip = false;

            foreach (var existing in rangesNoOverlap)
            {
                //If start and end is inside existing range, skip
                if (start >= existing.start && end <= existing.end)
                {
                    skip = true;
                    break;
                }

                //If start is inside existing range, but end is outside, move start to existing.end + 1
                if (start >= existing.start && start <= existing.end && end > existing.end)
                {
                    start = existing.end + 1;
                }
            }

            if (skip)
                continue;

            rangesNoOverlap.Add((start, end));
            Console.WriteLine($"{start} - {end}");
        }

        long sumOfNoOverlapRanges = 0;
        foreach (var range in rangesNoOverlap)
        {
            sumOfNoOverlapRanges += range.end - range.start + 1;
        }

        return (freshCount, sumOfNoOverlapRanges);
    }

    public string GetExampleInput()
    {
        return "3-5\n10-14\n16-20\n12-18\n\n1\n5\n8\n11\n17\n32";
    }
}