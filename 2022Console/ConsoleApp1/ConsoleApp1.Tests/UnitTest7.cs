using Xunit;
using ConsoleApp1;
namespace ConsoleApp1.Tests;

public class UnitTest7
{
    [Fact]
    public void Test7()
    {
        var e = new Event
        {
            Id = 1,
            AnimalId = 1,
            DayId = 1,
            EventText = "nothing",
            IsBorn = false,
            IsDie = false,
            WorkingShiftId = 1,
            EventTime = DateTime.ParseExact("19:59:40", "HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture)
        };

        var id = e.Id;
        var animalId = e.AnimalId;
        var dayId = e.DayId;
        var text = e.EventText;
        var born = e.IsBorn;
        var die = e.IsDie;
        var shiftId = e.WorkingShiftId;
        var hour = e.EventTime.Hour;
        var minute = e.EventTime.Minute;
        var second = e.EventTime.Second;

        Assert.Equal(1, id);
        Assert.Equal(1, animalId);
        Assert.Equal(1, dayId);
        Assert.Equal("nothing", text);
        Assert.False(born);
        Assert.False(die);
        Assert.Equal(1, shiftId);
        Assert.Equal(19, hour);
        Assert.Equal(59, minute);
        Assert.Equal(40, second);
    }
}