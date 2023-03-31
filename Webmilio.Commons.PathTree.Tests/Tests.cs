namespace Webmilio.Commons.PathTree.Tests;

[TestClass]
public class Tests
{
    [TestMethod]
    public void RootLevel()
    {
        var root = new Root<int>();

        const string Path = "test";
        const int Value = 1;

        root.Add(Path, Value);
        Assert.AreEqual(Value, root.Get(Path));
    }

    [TestMethod]
    public void BigAdd()
    {
        var root = new Root<int>();

        root.Add("test", 1);
        root.Add("test.x.y.z", 3);
        root.Add("test.x.abc", 5);

        Assert.IsTrue(true);
    }

    [TestMethod]
    public void OneLevel()
    {
        var root = new Root<int>();

        const string RootPath = "test";
        const string CombinedPath = RootPath + "." + RootPath; // Funky same identifier test.
        const int Value = 1;

        root.Add(CombinedPath, Value);
        Assert.AreEqual(0, root.Get(RootPath));
        Assert.AreEqual(Value, root.Get(CombinedPath));
    }
}