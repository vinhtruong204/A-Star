
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