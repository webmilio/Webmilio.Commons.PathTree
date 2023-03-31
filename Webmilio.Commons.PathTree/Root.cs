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

    public T Get(string path)
    {
        return Get(path, 0);
    }
}