using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WorkingCalendar.Infrastructure;

public class CalendarDbContextFactory : IDesignTimeDbContextFactory<CalendarDbContext>
{
    public CalendarDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CalendarDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=workingcalendar;Username=workingcalendar;Password=workingcalendar");
        return new CalendarDbContext(optionsBuilder.Options);
    }
}
