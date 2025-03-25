// Lưu trữ thông tin về 1 cạnh: đỉnh kề và chi phí
public class Edge
{
    public string Neighbor { get; set; }
    public int Cost { get; set; }
    public Edge(string neighbor, int cost)
    {
        Neighbor = neighbor;
        Cost = cost;
    }
}