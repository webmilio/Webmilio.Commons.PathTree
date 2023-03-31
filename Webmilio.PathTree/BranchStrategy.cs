using System.IO;
using System;
using System.Reflection;

namespace Webmilio.Commons.PathTree;

public abstract class BranchStrategy<T>
{
    public abstract Node<T> Transform(Leaf<T> source, Dictionary<string, Node<T>> children);

    public virtual T Get(string path, int index, char delimiter, Dictionary<string, Node<T>> children)
    {
        if (index + 1 > path.Length)
        {
            return default;
        }

        var nextIndex = path.IndexOf(delimiter, index + 1);

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

    protected virtual T GetTraversal(string path, int index, char delimiter, Dictionary<string, Node<T>> children)
    {
        var nextIndex = path.IndexOf(delimiter, index + 1);

        if (nextIndex < 0)
        {
            nextIndex = path.Length;
        }

        Node<T> child;

        if (children.TryGetValue(path[index..nextIndex], out child))
        {
            return child.Get(path, nextIndex + 1);
        }

        if (children.TryGetValue(Wildcard, out child))
        {
            return child.Get(path, nextIndex + 1);
        }

        return default;
    }

    public string Wildcard { get; init; } = "*";

    public class Grow : BranchStrategy<T>
    {
        public override Node<T> Transform(Leaf<T> source, Dictionary<string, Node<T>> children)
        {
            var branch = new Branch<T>(source.Parent, source.Segment);
            
            children.Remove(branch.Segment);

            source.Segment = SelfIdentifier;
            branch.AddNodeUnsafe(source);
            
            source.Path = branch.Path + source.Parent.Delimiter + source.Segment;

            branch.Parent.AddNodeUnsafe(branch);

            return branch;
        }

        public override T Get(string path, int index, char delimiter, Dictionary<string, Node<T>> children)
        {
            Node<T> child;

            if (index + 1 > path.Length)
            {
                if (children.TryGetValue(SelfIdentifier, out child))
                {
                    return child.Get(path + delimiter + SelfIdentifier, index + 1 + SelfIdentifier.Length);
                }

                return default;
            }

            return GetTraversal(path, index, delimiter, children);
        }

        public string SelfIdentifier { get; init; } = "self";
    }

    public class Squash : BranchStrategy<T>
    {
        public override Node<T> Transform(Leaf<T> source, Dictionary<string, Node<T>> children)
        {
            children.Remove(source.Segment);
            return source.Parent;
        }
    }
}