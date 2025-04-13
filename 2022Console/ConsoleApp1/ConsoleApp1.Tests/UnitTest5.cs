using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest5
{
    [Fact]
    public void Test5()
    {
        var babyAnimal = new BabyAnimal { Name = "Puchok", AnimalId = 1 };

        var name = babyAnimal.Name;
        var id = babyAnimal.AnimalId;

        Assert.Equal("Puchok", name);
        Assert.Equal(1, id);
    }
}
