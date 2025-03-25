

public class AStarAlgorithm
{
    private Dictionary<string, List<Edge>> graph;
    private Dictionary<string, int> heuristic; // Hàm heuristic h(v)
    private HashSet<string> closedSet; // Tập các đỉnh đã được kiểm tra
    private PriorityQueue<NodeRecord, int> openSet; // Tập các đỉnh chưa được kiểm tra (hàng đợi ưu tiên) 
    private Dictionary<string, NodeRecord> nodeRecords; // Lưu trữ thông tin về các đỉnh trong open set
    private string start; // Đỉnh bắt đầu
    private string goal; // Đỉnh đích

    public AStarAlgorithm()
    {
        graph = new Dictionary<string, List<Edge>>();
        heuristic = new Dictionary<string, int>();
        closedSet = new HashSet<string>();
        openSet = new PriorityQueue<NodeRecord, int>();
        nodeRecords = new Dictionary<string, NodeRecord>();

        #region Graph Input
        ReadInputFile("input.txt");
        #endregion

        Solve();
    }

    private void ReadInputFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        int index = 0;

        // Đọc heuristic
        while (!string.IsNullOrWhiteSpace(lines[index]) && lines[index].Split().Length == 2)
        {
            string[] parts = lines[index].Split();
            heuristic[parts[0]] = int.Parse(parts[1]);
            index++;
        }

        // Đọc danh sách cạnh
        while (!string.IsNullOrWhiteSpace(lines[index]) && lines[index].Split().Length == 3)
        {
            string[] parts = lines[index].Split();
            string from = parts[0], to = parts[1];
            int cost = int.Parse(parts[2]);

            if (!graph.ContainsKey(from))
                graph[from] = new List<Edge>();

            graph[from].Add(new Edge(to, cost));

            index++;
        }

        // Đọc điểm bắt đầu và điểm đích
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
        NodeRecord startNode = new NodeRecord(start, null, 0, heuristic[start]);

        openSet.Enqueue(startNode, startNode.F);

        while (openSet.Count > 0)
        {
            NodeRecord currentNode = openSet.Dequeue();

            closedSet.Add(currentNode.Node);

            if (currentNode.Node == goal)
            {
                Console.WriteLine("Path found!");
                // PrintPath(currentNode);
                return;
            }

            foreach (Edge edge in graph[currentNode.Node])
            {
                if (closedSet.Contains(edge.Neighbor))
                    continue;

                int g = currentNode.G + edge.Cost;
                int h = heuristic[edge.Neighbor];
                NodeRecord neighborNode = new NodeRecord(edge.Neighbor, currentNode.Node, g, h);

                if (!nodeRecords.ContainsKey(edge.Neighbor) || g < nodeRecords[edge.Neighbor].G)
                {
                    nodeRecords[edge.Neighbor] = neighborNode;
                    openSet.Enqueue(neighborNode, neighborNode.F);
                }
            }
        }
    }

    private void PrintPath(NodeRecord currentNode)
    {
        List<string> path = new List<string>();
        while (currentNode != null)
        {
            path.Add(currentNode.Node);

            if (currentNode.Parent == null) // Nếu đã đến start node thì dừng
                break;
            currentNode = nodeRecords[currentNode.Parent];
        }
        path.Reverse();
        Console.WriteLine("Path: " + string.Join(" -> ", path));
    }
}