

using System.Text;

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
        start = string.Empty;
        goal = string.Empty;

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
            graph[parts[0]] = new List<Edge>();
            index++;
        }

        // Đọc danh sách cạnh
        while (!string.IsNullOrWhiteSpace(lines[index]) && lines[index].Split().Length == 3)
        {
            string[] parts = lines[index].Split();
            string from = parts[0], to = parts[1];
            int cost = int.Parse(parts[2]);


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
        StringBuilder sb = new StringBuilder();

        // Định nghĩa độ rộng cột
        int col1Width = 5;   // TT
        int col2Width = 5;   // TTK
        int col3Width = 8;   // k(u, v)
        int col4Width = 8;   // h(v)
        int col5Width = 8;   // g(v)
        int col6Width = 8;   // f(v) = g(v) + h(v)
        int col7Width = 50;  // DSL


        // Ghi tiêu đề bảng
        sb.AppendLine($"{"TT".PadRight(col1Width)} | {"TTK".PadRight(col2Width)} | {"k(u, v)".PadRight(col3Width)} | {"h(v)".PadRight(col4Width)}" +
                      $" | {"g(v)".PadRight(col5Width)} | {"f(v)".PadRight(col6Width)} | {"DSL".PadRight(col7Width)}");
        sb.AppendLine(new string('-', col1Width + col2Width + col3Width + col4Width + col5Width + col6Width + col7Width + 15)); // Đường kẻ ngang

        NodeRecord startNode = new NodeRecord(start, null!, 0, heuristic[start]);
        openSet.Enqueue(startNode, startNode.F);

        while (openSet.Count > 0)
        {
            NodeRecord currentNode = openSet.Dequeue();
            closedSet.Add(currentNode.Node);

            if (currentNode.Node == goal)
            {
                Console.WriteLine("Path found!");
                PrintPath(currentNode, sb);
                // Ghi vào file
                File.WriteAllText("output.txt", sb.ToString());
                return;
            }

            bool isFirstLine = false;
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


                // Cập nhật DSL (Danh sách đỉnh trong Priority Queue)
                List<string> openSetList = openSet.UnorderedItems
                                                .OrderBy(n => n.Priority)
                                                .Select(n => $"{n.Element.Node}({n.Priority})")
                                                .ToList();

                string openSetStr = string.Join(", ", openSetList);

                sb.AppendLine($"{(!isFirstLine ? currentNode.Node.PadRight(col1Width) : " ".PadRight(col1Width))} | {edge.Neighbor.PadRight(col2Width)} | " +
                              $"{edge.Cost.ToString().PadRight(col3Width)} | {h.ToString().PadRight(col4Width)} | " +
                              $"{g.ToString().PadRight(col5Width)} | {neighborNode.F.ToString().PadRight(col6Width)} | " +
                              $"{openSetStr.PadRight(col7Width)}"); // Ghi DSL vào bảng
                isFirstLine = true;
            }

        }

        sb.AppendLine($"No path found from {start} to {goal}.");
        File.WriteAllText("output.txt", sb.ToString());
    }

    private void PrintPath(NodeRecord currentNode, StringBuilder sb)
    {
        List<string> path = new List<string>();
        int cost = currentNode.F;

        while (currentNode != null)
        {
            path.Add(currentNode.Node);

            if (currentNode.Parent == start)
            {
                path.Add(currentNode.Parent);
                break;
            }

            currentNode = nodeRecords[currentNode.Parent];
        }

        path.Reverse();
        sb.AppendLine("Path: " + string.Join(" -> ", path));
        sb.AppendLine("Cost: " + cost);
    }
}