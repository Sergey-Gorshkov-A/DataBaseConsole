using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var area = new Area { Area_name = "Area 1", Adress = "Pskov"};

        var name = area.Area_name;
        var adress = area.Adress;

        Assert.Equal("Area 1", name);
        Assert.Equal("Pskov", adress);
    }
}
