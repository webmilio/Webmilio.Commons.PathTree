namespace Webmilio.Commons.PathTree;

public abstract class Node<T>
{
    protected const char Delimiter = '.';

    internal abstract T Get(string path, int pathIndex);

    public Node<T> Parent { get; internal set; }
    public string FullName { get; internal set; }
}