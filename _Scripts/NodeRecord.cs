// Thông tin trong hàng đợi (open set)
public class NodeRecord : IComparable<NodeRecord>
{
    public string Node { get; set; }
    public string Parent { get; set; }  // Dùng để truy vết đường đi
    public int G { get; set; }          // g(v)
    public int H { get; set; }          // h(v)
    public int F => G + H;              // f(v) = g(v) + h(v)

    public NodeRecord(string node, string parent, int g, int h)
    {
        Node = node;
        Parent = parent;
        G = g;
        H = h;
    }

    // Để sắp xếp trong priority queue theo f
    public int CompareTo(NodeRecord? other)
    {
        return this.F.CompareTo(other?.F);
    }
}