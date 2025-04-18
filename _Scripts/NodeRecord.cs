
public class NodeRecord : IComparable<NodeRecord>
{
    public string Node { get; set; }
    public string Parent { get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F => G + H;

    public NodeRecord(string node, string parent, int g, int h)
    {
        Node = node;
        Parent = parent;
        G = g;
        H = h;
    }
    
    public int CompareTo(NodeRecord? other)
    {
        return this.F.CompareTo(other?.F);
    }
}