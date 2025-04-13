using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest8
{
    [Fact]
    public void Test8()
    {
        var employer = new Employer
        {
            Id = 1,
            Fio = "GorshkovSergeyAlbertovich",
            AreaId = 1,
        };

        var id = employer.Id;
        var name = employer.Fio;
        var areaId = employer.AreaId;

        Assert.Equal(1, id);
        Assert.Equal("GorshkovSergeyAlbertovich", name);
        Assert.Equal(1, areaId);
    }
}
