namespace aoc2025;

public class Day11 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        Dictionary<string, Node> nodes = new();
        nodes.Add("out", new Node() { Name = "out" });

        foreach (var line in puzzleInput)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var nodeName = line.Split(':')[0].Trim();
            nodes.Add(nodeName, new Node() { Name = nodeName });
        }

        foreach (var line in puzzleInput)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var nodeName = line.Split(':')[0].Trim();
            var connections = line.Split(':')[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var connection in connections)
            {
                nodes[nodeName].AddOutConnection(nodes[connection]);
            }
        }

        CalculateNumberOfWaysToOut(nodes);

        var part1 = nodes["you"].NumberOfWaysToOut;

        return (part1, 0);
    }

    private void CalculateNumberOfWaysToOut(Dictionary<string, Node> nodes)
    {
        Queue<Node> queue = new();
        var outNode = nodes["out"];
        foreach (var inConnection in outNode.InConnections.Values)
        {
            queue.Enqueue(inConnection);
        }

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            if (currentNode.NumberOfWaysToOut != -1)
                continue;

            GetNumberOfWaysToOut(currentNode);

            foreach (var inConnection in currentNode.InConnections.Values)
            {
                queue.Enqueue(inConnection);
            }
        }
    }

    private int GetNumberOfWaysToOut(Node node)
    {
        if (node.NumberOfWaysToOut != -1)
            return node.NumberOfWaysToOut;

        if (node.OutConnections.ContainsKey("out"))
        {
            node.NumberOfWaysToOut = 1;
            return 1;
        }

        int totalWays = 0;
        foreach (var outConnection in node.OutConnections.Values)
        {
            totalWays += GetNumberOfWaysToOut(outConnection);
        }

        node.NumberOfWaysToOut = totalWays;
        return totalWays;
    }

    private class Node
    {
        public string Name;
        public int NumberOfWaysToOut = -1;
        public Dictionary<string, Node> OutConnections = new();
        public Dictionary<string, Node> InConnections = new();

        public void AddOutConnection(Node other)
        {
            OutConnections.Add(other.Name, other);
            other.InConnections.Add(Name, this);
        }
    }

    public string GetExampleInput()
    {
        return
            "aaa: you hhh\nyou: bbb ccc\nbbb: ddd eee\nccc: ddd eee fff\nddd: ggg\neee: out\nfff: out\nggg: out\nhhh: ccc fff iii\niii: out\n";
    }
}