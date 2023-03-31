namespace Webmilio.Commons.PathTree;

public class Branch<T> : Node<T>
{
    protected Dictionary<string, Node<T>> children = new(StringComparer.OrdinalIgnoreCase);

    internal override T Get(string path, int index)
    {
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

    protected (int Index, Branch<T> Branch) GetDeepestBranch(string path, int index)
    {
        var nextIndex = path.IndexOf(Delimiter, index + 1);

        if (nextIndex > 0 &&
            children.TryGetValue(path[index..nextIndex], out var child) &&
            child is Branch<T> b)
        {
            return b.GetDeepestBranch(path, nextIndex);
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

        var subPath = path[index..nextIndex];
        var deepestBranch = GetDeepestBranch(path, index);

        if (deepestBranch.Branch == this)
        {
            if (nextIndex == path.Length) // We're making a leaf!
            {
                children.Add(subPath, new Leaf<T>(subPath, path, value, this));
            }
            else
            {
                var branch = new Branch<T>()
                {
                    FullName = subPath,
                    Parent = this
                };

                children.Add(subPath, branch);
                branch.Add(path, nextIndex + 1, value);
            }
        }
        else
        {
            deepestBranch.Branch.Add(path, nextIndex + 1, value);
        }
    }
}