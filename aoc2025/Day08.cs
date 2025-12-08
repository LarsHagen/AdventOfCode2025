namespace aoc2025;

public class Day08 : IAocDay
{
    public async Task<(object part1, object part2)> Run(List<string> puzzleInput)
    {
        List<JunctionBox> junctionBoxes = new();

        Console.WriteLine("Parse");

        foreach (var position in puzzleInput)
        {
            var split = position.Split(',');
            junctionBoxes.Add(new JunctionBox(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2])));
        }

        Console.WriteLine("Calculate distances");

        foreach (var junctionBox in junctionBoxes)
        {
            foreach (var other in junctionBoxes)
            {
                if (junctionBox == other)
                    continue;

                junctionBox.CalculateDistanceAndStore(other);
            }
        }

        int numberShortestConnectionsToFindPart1 = puzzleInput.Count == 20 ? 10 : 1000; //Only 10 for example input
        HashSet<(JunctionBox a, JunctionBox b)> connections = new();
        Dictionary<JunctionBox, Guid> boxByNetworkId = new();

        int part1 = 0;
        long part2 = 0;

        //for (int i = 0; i < numberShortestConnectionsToFind; i++)
        int i = 0;
        while (true)
        {

            Console.WriteLine("Find connection " + i);
            JunctionBox boxA = null;
            JunctionBox boxB = null;
            double shortestDistance = int.MaxValue;

            foreach (var junctionBox in junctionBoxes)
            {
                foreach (var (otherBox, distance) in junctionBox.DistanceToOtherBoxes)
                {
                    if (connections.Contains((junctionBox, otherBox)) || connections.Contains((otherBox, junctionBox)))
                        continue;

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        boxA = junctionBox;
                        boxB = otherBox;
                    }
                }
            }

            if (boxByNetworkId.ContainsKey(boxA) && boxByNetworkId.ContainsKey(boxB))
            {
                //Merge networks
                var networkIdA = boxByNetworkId[boxA];
                var networkIdB = boxByNetworkId[boxB];
                if (networkIdA != networkIdB)
                {
                    foreach (var boxedByNetwork in boxByNetworkId)
                    {
                        if (boxedByNetwork.Value == networkIdB)
                        {
                            boxByNetworkId[boxedByNetwork.Key] = networkIdA;
                        }
                    }
                }
            }
            else if (boxByNetworkId.TryGetValue(boxA, out Guid networkA))
            {
                boxByNetworkId[boxB] = networkA;
            }
            else if (boxByNetworkId.TryGetValue(boxB, out Guid networkB))
            {
                boxByNetworkId[boxA] = networkB;
            }
            else
            {
                Guid newNetworkId = Guid.NewGuid();
                boxByNetworkId[boxA] = newNetworkId;
                boxByNetworkId[boxB] = newNetworkId;
            }

            connections.Add((boxA, boxB));

            i++;

            if (i == numberShortestConnectionsToFindPart1)
            {
                var netWorksBySize = boxByNetworkId
                    .GroupBy(kv => kv.Value)
                    .Select(g => g.Count())
                    .OrderByDescending(size => size)
                    .ToList();

                part1 = netWorksBySize[0] * netWorksBySize[1] * netWorksBySize[2];
            }

            if (i > numberShortestConnectionsToFindPart1)
            {
                var numberOfNetworks = boxByNetworkId
                    .GroupBy(kv => kv.Value)
                    .Count();
                var junctionBoxesNotInNetwork = junctionBoxes.Count - boxByNetworkId.Count;

                if (junctionBoxesNotInNetwork == 0 && numberOfNetworks == 1)
                {
                    Console.WriteLine("Everything is connected in one network. Last connection made: ");
                    Console.WriteLine($"Box A: ({boxA.X}, {boxA.Y}, {boxA.Z})");
                    Console.WriteLine($"Box B: ({boxB.X}, {boxB.Y}, {boxB.Z})");
                    part2 = boxA.X * boxB.X;
                    break;
                }
            }
        }

        Console.WriteLine("Order networks by size");

        return (part1, part2);
    }

    private class JunctionBox
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public JunctionBox(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Dictionary<JunctionBox, double> DistanceToOtherBoxes { get; set; } = new();

        public void CalculateDistanceAndStore(JunctionBox otherBox)
        {
            //int distance = Math.Abs(X - otherBox.X) + Math.Abs(Y - otherBox.Y) + Math.Abs(Z - otherBox.Z);
            double straightLineDistance = Math.Sqrt(
                Math.Pow(X - otherBox.X, 2) +
                Math.Pow(Y - otherBox.Y, 2) +
                Math.Pow(Z - otherBox.Z, 2)
            );
            DistanceToOtherBoxes[otherBox] = straightLineDistance;
        }
    }

    public string GetExampleInput()
    {
        return
            "162,817,812\n57,618,57\n906,360,560\n592,479,940\n352,342,300\n466,668,158\n542,29,236\n431,825,988\n739,650,466\n52,470,668\n216,146,977\n819,987,18\n117,168,530\n805,96,715\n346,949,466\n970,615,88\n941,993,340\n862,61,35\n984,92,344\n425,690,689";
    }
}