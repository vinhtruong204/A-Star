

using System.Text;

public class AStarAlgorithm
{
    private Dictionary<string, List<Edge>> graph;
    private Dictionary<string, int> heuristic;
    private PriorityQueue<NodeRecord, int> priorityQueue;
    private List<NodeRecord> nodeRecords;
    private string start;
    private string goal;

    public AStarAlgorithm()
    {
        graph = new Dictionary<string, List<Edge>>();
        heuristic = new Dictionary<string, int>();
        priorityQueue = new PriorityQueue<NodeRecord, int>();
        nodeRecords = new List<NodeRecord>();
        start = string.Empty;
        goal = string.Empty;

        ReadInputFile("input.txt");

        Solve();
    }

    private void ReadInputFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        int index = 0;

        while (!string.IsNullOrWhiteSpace(lines[index]) && lines[index].Split().Length == 2)
        {
            string[] parts = lines[index].Split();
            heuristic[parts[0]] = int.Parse(parts[1]);
            graph[parts[0]] = new List<Edge>();
            index++;
        }

        while (!string.IsNullOrWhiteSpace(lines[index]) && lines[index].Split().Length == 3)
        {
            string[] parts = lines[index].Split();
            string from = parts[0], to = parts[1];
            int cost = int.Parse(parts[2]);

            graph[from].Add(new Edge(to, cost));

            index++;
        }

        start = lines[index].Trim();
        goal = lines[index + 1].Trim();

    }

    public void PrintGraph()
    {
        Console.WriteLine("Heuristic:");
        foreach (KeyValuePair<string, int> kvp in heuristic)
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");

        Console.WriteLine("\nGraph:");
        foreach (KeyValuePair<string, List<Edge>> kvp in graph)
        {
            Console.Write($"{kvp.Key}: ");
            foreach (Edge edge in graph[kvp.Key])
                Console.Write($"({edge.Neighbor}, {edge.Cost}) ");
            Console.WriteLine();
        }

        Console.WriteLine($"\nStart: {start}");
        Console.WriteLine($"Goal: {goal}");
    }

    public void Solve()
    {
        StringBuilder sb = new();

        int col1Width = 5;
        int col2Width = 5;
        int col3Width = 8;
        int col4Width = 8;
        int col5Width = 8;
        int col6Width = 8;
        int col7Width = 50;

        sb.AppendLine($"{"TT".PadRight(col1Width)} | " +
                      $"{"TTK".PadRight(col2Width)} | " +
                      $"{"k(u, v)".PadRight(col3Width)} | " +
                      $"{"h(v)".PadRight(col4Width)} | " +
                      $"{"g(v)".PadRight(col5Width)} | " +
                      $"{"f(v)".PadRight(col6Width)} | " +
                      $"{"DSL".PadRight(col7Width)}");
        sb.AppendLine(new string('-', col1Width + col2Width + col3Width + col4Width + col5Width + col6Width + col7Width + 15));

        NodeRecord startNode = new NodeRecord(start, null!, 0, heuristic[start]);
        priorityQueue.Enqueue(startNode, startNode.F);

        while (priorityQueue.Count > 0)
        {
            NodeRecord currentNode = priorityQueue.Dequeue();
            string currentNodeInfor = currentNode.Node + "(" + currentNode.F + ")";

            if (currentNode.Node == goal)
            {
                sb.AppendLine($"{currentNodeInfor.PadRight(col1Width)} | TTKT/Stop ");
                PrintPath(currentNode, sb);

                File.WriteAllText("output.txt", sb.ToString());
                return;
            }

            bool isFirstLine = false;
            int currentEdgeCount = 0;
            int totalEdgeCount = graph[currentNode.Node].Count;

            foreach (Edge edge in graph[currentNode.Node])
            {
                currentEdgeCount++;

                int g = currentNode.G + edge.Cost;
                int h = heuristic[edge.Neighbor];
                NodeRecord neighborNode = new NodeRecord(edge.Neighbor, currentNode.Node, g, h);

                nodeRecords.Add(neighborNode);
                priorityQueue.Enqueue(neighborNode, neighborNode.F);

                if (currentEdgeCount < totalEdgeCount)
                {
                    sb.AppendLine($"{(!isFirstLine ? currentNodeInfor.PadRight(col1Width) : " ".PadRight(col1Width))} | " +
                                  $"{edge.Neighbor.PadRight(col2Width)} | " +
                                  $"{edge.Cost.ToString().PadRight(col3Width)} | " +
                                  $"{h.ToString().PadRight(col4Width)} | " +
                                  $"{g.ToString().PadRight(col5Width)} | " +
                                  $"{neighborNode.F.ToString().PadRight(col6Width)} | " +
                                  $"{" ".PadRight(col7Width)}");
                }
                else
                {
                    sb.Append($"{(!isFirstLine ? currentNodeInfor.PadRight(col1Width) : " ".PadRight(col1Width))} | " +
                                  $"{edge.Neighbor.PadRight(col2Width)} | " +
                                  $"{edge.Cost.ToString().PadRight(col3Width)} | " +
                                  $"{h.ToString().PadRight(col4Width)} | " +
                                  $"{g.ToString().PadRight(col5Width)} | " +
                                  $"{neighborNode.F.ToString().PadRight(col6Width)} | ");
                }
                
                isFirstLine = true;
            }

            List<string> openSetList = priorityQueue.UnorderedItems
                                                .OrderBy(n => n.Priority)
                                                .Select(n => $"{n.Element.Node}({n.Priority})")
                                                .ToList();

            string openSetStr = string.Join(", ", openSetList);

            sb.AppendLine(openSetStr.PadRight(col7Width));
            sb.AppendLine(new string('-', col1Width + col2Width + col3Width + col4Width + col5Width + col6Width + col7Width + 15));

        }

        sb.AppendLine($"No path found from {start} to {goal}.");
        File.WriteAllText("output.txt", sb.ToString());
    }

    private void PrintPath(NodeRecord? currentNode, StringBuilder sb)
    {
        List<string> path = new List<string>();
        int cost = currentNode!.F;

        while (currentNode != null)
        {
            path.Add(currentNode.Node);

            if (currentNode.Parent == start)
            {
                path.Add(currentNode.Parent);
                break;
            }

            currentNode = nodeRecords.FirstOrDefault(n => n.Node == currentNode.Parent);
        }

        path.Reverse();
        sb.AppendLine("Path: " + string.Join(" -> ", path));
        sb.AppendLine("Cost: " + cost);
    }
}