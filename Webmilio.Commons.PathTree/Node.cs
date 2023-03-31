namespace Webmilio.Commons.PathTree;

public abstract class Node<T>
{
    protected const char Delimiter = '.';

    internal abstract T Get(string path, int pathIndex);

    public Branch<T> Parent { get; internal set; }
    public string Path { get; internal set; }
}