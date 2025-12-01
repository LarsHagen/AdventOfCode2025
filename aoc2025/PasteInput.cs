namespace aoc2025;

public class PasteInput
{
    public static async Task<List<string>> PasteInputPrompt()
    {
        Console.WriteLine("Paste puzzle input and press enter or leave blank to use sample:");
        var lines = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            
            lines.Add(line);
        }
        
        return lines;
    }
}