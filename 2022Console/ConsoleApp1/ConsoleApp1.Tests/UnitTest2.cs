using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest2
{
    [Fact]
    public void Test2()
    {
        var adultAnimal = new AdultAnimal { Name = "Pushok", AnimalId = 1 };

        var name = adultAnimal.Name;

        Assert.Equal("Pushok", name);
    }
}
