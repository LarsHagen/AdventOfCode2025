using Spectre.Console;

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


        long part1 = 0;
        if (nodes.ContainsKey("you"))
            part1 = CalculateNumberOfWaysToNode(nodes, nodes["you"], nodes["out"]);
        else
            AnsiConsole.MarkupLine("[yellow]Warning: 'you' node not found in input! If you are using sample input make sure GetExampleInput returns sample for part 1[/]");

        long part2 = 0;
        if (nodes.ContainsKey("svr"))
        {
            var svrToDac = CalculateNumberOfWaysToNode(nodes, nodes["svr"], nodes["dac"], nodes["fft"]);
            var svrToFft = CalculateNumberOfWaysToNode(nodes, nodes["svr"], nodes["fft"], nodes["dac"]);
            var fftToDac = CalculateNumberOfWaysToNode(nodes, nodes["fft"], nodes["dac"], nodes["svr"]);
            var dacToFft = CalculateNumberOfWaysToNode(nodes, nodes["dac"], nodes["fft"], nodes["svr"]);
            var dacToOut = CalculateNumberOfWaysToNode(nodes, nodes["dac"], nodes["out"], nodes["fft"]);
            var fftToOut = CalculateNumberOfWaysToNode(nodes, nodes["fft"], nodes["out"], nodes["dac"]);

            var svrFftDacOut = svrToFft * fftToDac * dacToOut;
            var svrDacFftOut = svrToDac * dacToFft * fftToOut;
            part2 = svrFftDacOut + svrDacFftOut;
        }
        else
            AnsiConsole.MarkupLine("[yellow]Warning: 'svr' node not found in input! If you are using sample input make sure GetExampleInput returns sample for part 2[/]");

        //var part1 = nodes["you"].NumberOfWaysToOut;

        return (part1, part2);
    }

    private long CalculateNumberOfWaysToNode(Dictionary<string, Node> nodes, Node from, Node to, Node avoid = null)
    {
        foreach (var node in nodes)
        {
            node.Value.NumberOfWaysToOut = -1;
        }

        Queue<Node> queue = new();
        foreach (var inConnection in to.InConnections.Values)
        {
            queue.Enqueue(inConnection);
        }

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            if (currentNode.NumberOfWaysToOut != -1)
                continue;

            GetNumberOfWaysToNode(currentNode, to, avoid);

            foreach (var inConnection in currentNode.InConnections.Values)
            {
                if (inConnection == avoid)
                    continue;

                queue.Enqueue(inConnection);
            }
        }

        return Math.Max(from.NumberOfWaysToOut, 0);
    }

    private long GetNumberOfWaysToNode(Node node, Node to, Node avoid)
    {
        if (node.NumberOfWaysToOut != -1)
            return node.NumberOfWaysToOut;

        if (node.OutConnections.ContainsKey(to.Name))
        {
            node.NumberOfWaysToOut = 1;
            return 1;
        }

        long totalWays = 0;
        foreach (var outConnection in node.OutConnections.Values)
        {
            if (outConnection == avoid)
                continue;

            totalWays += GetNumberOfWaysToNode(outConnection, to, avoid);
        }

        node.NumberOfWaysToOut = totalWays;
        return totalWays;
    }

    private class Node
    {
        public string Name;
        public long NumberOfWaysToOut = -1;
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
        //Part 1
        //return "aaa: you hhh\nyou: bbb ccc\nbbb: ddd eee\nccc: ddd eee fff\nddd: ggg\neee: out\nfff: out\nggg: out\nhhh: ccc fff iii\niii: out\n";

        //Part 2
        return "svr: aaa bbb\naaa: fft\nfft: ccc\nbbb: tty\ntty: ccc\nccc: ddd eee\nddd: hub\nhub: fff\neee: dac\ndac: fff\nfff: ggg hhh\nggg: out\nhhh: out";
    }
}