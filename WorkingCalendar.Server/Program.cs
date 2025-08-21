using System.Globalization;
using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

using WorkingCalendar.Application;
using WorkingCalendar.Infrastructure;
using WorkingCalendar.Infrastructure.Services;
using WorkingCalendar.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CalendarRepositoryOptions>(builder.Configuration.GetSection("CalendarData"));
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<CalendarDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<ICalendarRepository, DbCalendarRepository>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.Configure<CalendarUpdateOptions>(builder.Configuration.GetSection("CalendarUpdate"));
builder.Services.AddHttpClient<GitHubCalendarDataUpdater>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("WorkingCalendar.Server");
});
builder.Services.AddSingleton<ICalendarDataUpdater, GitHubCalendarDataUpdater>();
builder.Services.AddHostedService<CalendarUpdateWorker>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddHealthChecks();
builder.Services.AddScoped<DatabaseInitializer>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.UseHealthChecks("/hc");
app.UseDefaultFiles();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.MapGet("/CheckDayWorkingCalendar", async (string data, string days, ICalendarService service) =>
{
    var date = DateTime.ParseExact(data, "dd.MM.yyyy", CultureInfo.InvariantCulture);
    var result = await service.CheckDayAsync(date, int.Parse(days));
    return Results.Ok(result);
}).WithName("CheckDayWorkingCalendar");

app.MapGet("/GetYearWorkingCalendar", async (string year, string type, string days, ICalendarService service) =>
{
    var result = await service.GetYearSqlAsync(int.Parse(year), type, int.Parse(days));
    return Results.Ok(result);
}).WithName("GetYearWorkingCalendar");
app.MapFallbackToFile("index.html");
app.Run();
