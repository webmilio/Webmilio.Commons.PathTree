namespace Webmilio.Commons.PathTree;

public class Root<T> : Branch<T>
{
    public Root()
    {
        Path = "";
    }

    public void Add(string path, T value)
    {
        Add(path, 0, value);
    }

    public new void Remove(string path)
    {
        base.Remove(path);
    }

    public T Get(string path)
    {
        return Get(path, 0);
    }
}