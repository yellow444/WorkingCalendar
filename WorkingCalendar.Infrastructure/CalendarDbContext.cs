using Microsoft.EntityFrameworkCore;

namespace WorkingCalendar.Infrastructure;

public class CalendarDbContext : DbContext
{
    public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
    {
    }

    public DbSet<CalendarEntry> CalendarEntries => Set<CalendarEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<CalendarEntry>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new { e.Year, e.Culture }).IsUnique();
            builder.Property(e => e.Culture).IsRequired();
            builder.Property(e => e.Xml).IsRequired();
        });
    }
}
