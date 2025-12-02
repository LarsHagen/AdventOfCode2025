using Spectre.Console;

namespace aoc2025;

public class Day02 : IAocDay
{
    public Task<(string part1, string part2)> Run(List<string> puzzleInput)
    {
        string[] rangesRaw = puzzleInput[0].Split(',');
        List<(long start, long end)> ranges = new();
        long highest = 0;
        foreach (var s in rangesRaw)
        {
            var low = long.Parse(s.Split('-')[0]);
            var high = long.Parse(s.Split('-')[1]);
            ranges.Add((low, high));
            if (high > highest)
                highest = high;
        }

        var invalidPart1 = GetAllInValidPart1(highest);
        var inValidPart2 = GetAllInValidPart2(highest);

        long combinedPart1 = 0;
        long combinedPart2 = 0;

        var reportTable = new Table();
        reportTable.Border(TableBorder.Rounded);
        reportTable.AddColumn("Range");
        reportTable.Columns[0].Centered();
        reportTable.AddColumn("Part 1 value");
        reportTable.Columns[1].RightAligned();
        reportTable.AddColumn("Part 2 value");
        reportTable.Columns[2].RightAligned();
        
        
        foreach ((long start, long end) range in ranges)
        {
            
            for (long i = range.start; i <= range.end; i++)
            {
                if (invalidPart1.Contains(i))
                {
                    combinedPart1 += i;
                }
                if (inValidPart2.Contains(i))
                {
                    combinedPart2 += i;
                }
            }
            
            reportTable.AddRow($"{range.start} -> {range.end}", combinedPart1.ToString(), combinedPart2.ToString());
        }
        
        AnsiConsole.Write(reportTable);
        
        return Task.FromResult<(string part1, string part2)>((combinedPart1.ToString(),combinedPart2.ToString()));
    }

    private HashSet<long> GetAllInValidPart1(long highest)
    {
        int highestLength = highest.ToString().Length;
        int maxHalfPattern = int.Parse(new string('9', highestLength /2));
        
        HashSet<string> invalid = new();
        for (int a = 1; a <= maxHalfPattern; a++)
        {
            var asString = a.ToString();
            invalid.Add(asString + asString);
        }

        HashSet<long> result = new();
        foreach (var i in invalid)
        {
            result.Add(long.Parse(i));
        }
        
        return result;
    }
    
    private HashSet<long> GetAllInValidPart2(long highest)
    {
        int highestLength = highest.ToString().Length;
        int maxHalfPattern = int.Parse(new string('9', highestLength /2));
        
        HashSet<string> invalid = new();
        for (int a = 1; a <= maxHalfPattern; a++)
        {
            var asString = a.ToString();
            
            for (int numSubStrings = 2; numSubStrings <= maxHalfPattern; numSubStrings++)
            {
                var patternAsString = asString;

                for (int repeat = 1; repeat < numSubStrings; repeat++)
                {
                    patternAsString += asString;
                }

                if (patternAsString.Length > highestLength)
                    break;
                
                invalid.Add(patternAsString);
            }
        }

        HashSet<long> result = new();
        foreach (var i in invalid)
        {
            result.Add(long.Parse(i));
        }
        
        return result;
    }

    public string GetExampleInput()
    {
        return
            "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";
    }
}