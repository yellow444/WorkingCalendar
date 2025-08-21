using WorkingCalendar.Domain;

namespace WorkingCalendar.Server.Tests;

public class CalendarDomainTests
{
    [Fact]
    public void DescribeDay_ReturnsHolidayTitle()
    {
        const string xml = "<calendar><holidays><holiday id='1' title='Test'/></holidays><days><day d='01.01' h='1'/></days></calendar>";
        var calendar = Calendar.FromXml(xml);
        var result = calendar.DescribeDay(new DateTime(2024,1,1), 5);
        Assert.Equal("Test", result);
    }
}
