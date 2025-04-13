using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest4
{
    [Fact]
    public void Test4()
    {
        var aviary = new Aviary { Id = 1, AviaryName = "Zone 1", AreaId = 1};

        var name = aviary.AviaryName;
        var id = aviary.Id;
        var area_id = aviary.AreaId;

        Assert.Equal("Zone 1", name);
        Assert.Equal(1, id);
        Assert.Equal(1, area_id);
    }
}
