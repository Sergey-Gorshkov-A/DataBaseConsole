using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest3
{
    [Fact]
    public void Test3()
    {
        var animal = new Animal { Id = 1, TypeOfAnimal = "Tiger", AviaryId = 1, Status = "live"};

        var type = animal.TypeOfAnimal;
        var status = animal.Status;

        Assert.Equal("Tiger", type);
        Assert.Equal("live", status);
    }
}
