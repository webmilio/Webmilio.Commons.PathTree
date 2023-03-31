namespace Webmilio.Commons.PathTree;

public class Leaf<T> : Node<T>
{
    public Leaf(string path, string fullPath, T value, Branch<T> parent)
    {
        Path = path;
        FullName = fullPath;

        Value = value;
        Parent = parent;
    }

    internal override T Get(string path, int pathIndex)
    {
        return Value;
    }

    public T Value { get; private set; }

    public string Path { get; private set; }
}