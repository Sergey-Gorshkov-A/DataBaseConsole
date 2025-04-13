using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest10
{
    [Fact]
    public void Test10()
    {
        var user = new User
        {
            Id = 1,
            Login = "user",
            Password = "04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb",
        };

        var id = user.Id;
        var name = user.Login;
        var password = user.Password;

        Assert.Equal(1, id);
        Assert.Equal("user", name);
        Assert.Equal("04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb", password);
    }
}
