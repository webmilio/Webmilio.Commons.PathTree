using System.Collections.ObjectModel;

namespace Webmilio.PathTree;

public class Branch<T> : Node<T>
{
    protected Dictionary<string, Node<T>> children = new(StringComparer.OrdinalIgnoreCase);

    public Branch()
    {
        Children = new ReadOnlyDictionary<string, Node<T>>(children);
    }

    public Branch(Branch<T> parent, string segment) : this()
    {
        Delimiter = parent.Delimiter;
        Segment = segment;

        if (!string.IsNullOrWhiteSpace(parent.Path))
        {
            Path = parent.Path + Delimiter;
        }

        Path += Segment;
        Parent = parent;

        Strategy = parent.Strategy;
    }

    internal override T Get(string path, int index)
    {
        return Strategy.Get(path, index, Delimiter, children);
    }

    protected virtual (int Index, Node<T> Node) GetDeepestNode(string path, int index)
    {
        var nextIndex = path.IndexOf(Delimiter, index + 1);

        if (nextIndex > 0 && children.TryGetValue(path[index..nextIndex], out var child))
        {
            if (child is Branch<T> b)
            {
                return b.GetDeepestNode(path, nextIndex);
            }

            return (index, child);
        }

        return (index, this);
    }

    protected void Add(string path, int index, T value)
    {
        var nextIndex = path.IndexOf(Delimiter, index);

        if (nextIndex < 0)
        {
            nextIndex = path.Length;
        }

        var segment = path[index..nextIndex];
        var deepestNode = GetDeepestNode(path, index);

        bool loop = true;
        while (loop == true)
        {
            loop = false;

            if (deepestNode.Node == this)
            {
                if (nextIndex == path.Length) // We're making a leaf!
                {
                    AddNodeUnsafe(new Leaf<T>(segment, path, value, this));
                }
                else
                {
                    var branch = new Branch<T>(this, segment);

                    AddNodeUnsafe(branch);
                    branch.Add(path, nextIndex + 1, value);
                }
            }
            else
            {
                if (deepestNode.Node is Branch<T> b)
                {
                    b.Add(path, nextIndex + 1, value);
                }
                else if (deepestNode.Node is Leaf<T> l) // Since it's a Leaf<T>, we need remove it since Branches can't have values (should I add that?)
                {
                    deepestNode.Node = Strategy.Transform(l, children);

                    loop = true; // Refactor this.
                }
            }
        }
    }

    public void AddNodeUnsafe(Node<T> node)
    {
        children.Add(node.Segment, node);
        node.Parent = this;
    }

    protected Node<T> Remove(string path)
    {
        if (children.TryGetValue(path, out var node))
        {
            node.Parent = null;
            children.Remove(path);

            return node;
        }

        return null;
    }

    public char Delimiter { get; init; } = '.';

    public BranchStrategy<T> Strategy { get; init; } = new BranchStrategy<T>.Grow();

    public ReadOnlyDictionary<string, Node<T>> Children { get; }
}