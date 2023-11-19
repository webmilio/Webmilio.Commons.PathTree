using Webmilio.PathTree;

namespace Webmilio.PathTree.Tests;

[TestClass]
public class Tests
{
    [TestMethod]
    public void RootLevel()
    {
        var root = new Root<int>();

        root.Add("test", 1);
        Assert.AreEqual(1, root.Get("test"));
    }

    [TestMethod]
    public void Wildcard()
    {
        var root = new Root<int>();

        root.Add("test.x.*", 1);
        root.Add("test.y", 2);

        Assert.AreEqual(1, root.Get("test.x.123123123.za"));
        Assert.AreEqual(1, root.Get("test.x.y"));
        Assert.AreEqual(2, root.Get("test.y"));
    }

    [TestMethod]
    public void LeafSquash()
    {
        var root = new Root<int>()
        {
            Strategy = new BranchStrategy<int>.Squash()
        };

        root.Add("test", 1);
        root.Add("test.a", 2);

        Assert.AreEqual(0, root.Get("test"));
        Assert.AreEqual(2, root.Get("test.a"));
    }

    [TestMethod]
    public void LeafGrow()
    {
        var root = new Root<int>()
        {
            Strategy = new BranchStrategy<int>.Grow()
        };

        root.Add("test", 1);
        root.Add("test.a", 2);

        Assert.AreEqual(1, root.Get("test"));
        Assert.AreEqual(2, root.Get("test.a"));
    }

    [TestMethod]
    public void SameName()
    {
        var root = new Root<int>();
        root.Add("test.test", 1);

        Assert.AreEqual(0, root.Get("test"));
        Assert.AreEqual(1, root.Get("test.test"));
    }

    [TestMethod]
    public void MultiNamespaceSquash()
    {
        var root = new Root<int>()
        {
            Strategy = new BranchStrategy<int>.Squash()
        };

        var paths = new Dictionary<string, (int Actual, int Expected)>()
        {
            { "root.weather.canada",                        (100, 0)    },
            { "root.weather.canada.quebec",                 (101, 101)  }, // This destroys the canada node.

            { "root.weather.america.iowa",                  (500, 500)  },
            { "root.weather.america.california",            (501, 501)  },
            { "root.weather.america.wisconsin",             (502, 0)    },

            { "root.weather.america.wisconsin.somecity",    (503, 503)  } // This destroys the wisconsin node.
        };

        foreach (var items in paths)
        {
            root.Add(items.Key, items.Value.Actual);
        }

        foreach (var item in paths)
        {
            Assert.AreEqual(item.Value.Expected, root.Get(item.Key));
        }
    }

    [TestMethod]
    public void MultiNamespaceGrow()
    {
        var root = new Root<int>()
        {
            Strategy = new BranchStrategy<int>.Grow()
        };

        var paths = new Dictionary<string, (int Actual, int Expected)>()
        {
            { "root.weather.canada",                        (100, 100)  },
            { "root.weather.canada.quebec",                 (101, 101)  }, // This destroys the canada node.

            { "root.weather.canada.ontario.*",              (150, 150)  },
            { "root.weather.canada.ontario.kingston",       (0, 150)    },
            { "root.weather.canada.ontario.toronto",        (0, 150)    },

            { "root.weather.america.iowa",                  (500, 500)  },
            { "root.weather.america.california",            (501, 501)  },
            { "root.weather.america.wisconsin",             (502, 502)  },

            { "root.weather.america.wisconsin.somecity",    (503, 503)  } // This destroys the wisconsin node.
        };

        foreach (var pair in paths)
        {
            if (pair.Value.Actual > 0)
            {
                root.Add(pair.Key, pair.Value.Actual);
            }
        }

        foreach (var pair in paths)
        {
            Assert.AreEqual(pair.Value.Expected, root.Get(pair.Key));
        }
    }
}