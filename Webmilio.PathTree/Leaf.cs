namespace Webmilio.PathTree;

public class Leaf<T> : Node<T>
{
    public Leaf(string segment, string path, T value, Branch<T> parent)
    {
        Segment = segment;
        Path = path;

        Value = value;
        Parent = parent;
    }

    internal override T Get(string path, int pathIndex)
    {
        return Value;
    }

    public override string ToString()
    {
        return $"{base.ToString()} -> {Value}";
    }

    public T Value { get; private set; }
}