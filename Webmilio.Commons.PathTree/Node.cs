namespace Webmilio.Commons.PathTree;

public abstract class Node<T>
{
    internal abstract T Get(string path, int pathIndex);

    public override string ToString()
    {
        return $"{GetType().Name}: {Path}";
    }

    public Branch<T> Parent { get; internal set; }

    public string Segment { get; internal set; }
    public string Path { get; internal set; }
}