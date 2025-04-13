using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest9
{
    [Fact]
    public void Test9()
    {
        var shift = new WorkingShift
        {
            Id = 1,
            EmployerId = 1,
            TimeBegin = DateTime.ParseExact("10:15:40", "HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture),
            TimeEnd = DateTime.ParseExact("20:15:20", "HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture)
        };

        var id = shift.Id;
        var employerId = shift.EmployerId;
        var begin_h = shift.TimeBegin.Hour;
        var end_h = shift.TimeEnd.Hour;
        var begin_m = shift.TimeBegin.Minute;
        var end_m = shift.TimeEnd.Minute;
        var begin_s = shift.TimeBegin.Second;
        var end_s = shift.TimeEnd.Second;

        Assert.Equal(1, id);
        Assert.Equal(1, employerId);
        Assert.Equal(10, begin_h);
        Assert.Equal(20, end_h);
        Assert.Equal(15, begin_m);
        Assert.Equal(15, end_m);
        Assert.Equal(40, begin_s);
        Assert.Equal(20, end_s);
    }
}
