// See https://aka.ms/new-console-template for more information
using Webmilio.Commons.PathTree;

Console.WriteLine("Hello, World!");

var root = new Root<int>()
{
    Strategy = new BranchStrategy<int>.Grow()
};

var paths = new Dictionary<string, int>()
{
    { "root.weather.canada", 100 },
    { "root.weather.canada.quebec", 101 }, // This destroys the canada node.

    { "root.weather.america.iowa", 500 },
    { "root.weather.america.california", 501 },
    { "root.weather.america.wisconsin", 502 },

    { "root.weather.america.wisconsin.somecity", 503 } // This destroys the wisconsin node.
};

foreach (var items in paths) root.Add(items.Key, items.Value);

void Print<T>(Node<T> node)
{
    Console.Write(node.Path);

    if (node is Leaf<T> l)
        Console.Write($": {l.Value}");

    Console.WriteLine();

    if (node is Branch<T> b)
        foreach (var (_, child) in b.Children)
            Print(child);
}

Print(root);