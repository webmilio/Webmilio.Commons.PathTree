namespace Webmilio.Commons.PathTree;

public class Branch<T> : Node<T>
{
    protected Dictionary<string, Node<T>> children = new(StringComparer.OrdinalIgnoreCase);

    internal override T Get(string path, int index)
    {
        if (index + 1 > path.Length)
        {
            return default;
        }

        var nextIndex = path.IndexOf(Delimiter, index + 1);

        if (nextIndex < 0)
        {
            nextIndex = path.Length;
        }

        if (children.TryGetValue(path[index..nextIndex], out var child))
        {
            return child.Get(path, nextIndex + 1);
        }

        return default;
    }

    protected (int Index, Node<T> Node) GetDeepestNode(string path, int index)
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

        Add:
        if (deepestNode.Node == this)
        {
            if (nextIndex == path.Length) // We're making a leaf!
            {
                children.Add(segment, new Leaf<T>(segment, path, value, this));
            }
            else
            {
                var branch = new Branch<T>()
                {
                    Path = segment,
                    Parent = this
                };

                children.Add(segment, branch);
                branch.Add(path, nextIndex + 1, value);
            }
        }
        else
        {
            if (deepestNode.Node is Branch<T> b)
            {
                b.Add(path, nextIndex + 1, value);
            }
            else if (deepestNode.Node is Leaf<T>) // Since it's a Leaf<T>, we need remove it since Branches can't have values (should I add that?)
            {
                children.Remove(segment);
                deepestNode.Node = this;

                goto Add; // Refactor this.
            }
        }
    }

    protected void Remove(string path)
    {
        children.Remove(path);
    }
}