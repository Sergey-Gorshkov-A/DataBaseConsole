using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest6
{
    [Fact]
    public void Test6()
    {
        var day = new Day
        {
            Id = 1,
            Date = DateTime.ParseExact("2025-04-12", "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture)
        };

        var id = day.Id;
        var year = day.Date.Year;
        var month = day.Date.Month;
        var date = day.Date.Day;
        
        Assert.Equal(1, id);
        Assert.Equal(2025, year);
        Assert.Equal(04, month);
        Assert.Equal(12, date);
    }
}
