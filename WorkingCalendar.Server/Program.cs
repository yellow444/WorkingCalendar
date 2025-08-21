using System.Globalization;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WorkingCalendar.Application;
using WorkingCalendar.Infrastructure;
using WorkingCalendar.Infrastructure.Services;
using WorkingCalendar.Server;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.Configure<CalendarRepositoryOptions>(builder.Configuration.GetSection("CalendarData"));
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<CalendarDbContext>(options => options.UseNpgsql(connectionString));
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

// ▶ ВКЛЮЧАЕМ контроллеры (иначе API не сматчатся и уйдут в SPA fallback)
builder.Services.AddControllers();

builder.Services.AddScoped<DatabaseInitializer>();

var app = builder.Build();

// --- Init DB ---
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

// --- Pipeline ---
app.UseHealthChecks("/hc");

// Раздача SPA (работает всегда)
app.UseDefaultFiles();
app.UseStaticFiles();

// Swagger — только при Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
}

// ▶ Маршруты API (ставим до SPA fallback)
app.MapControllers();

// Тестовый ping в нужной базе пути (можете удалить)
app.MapGet("/WorkingCalendar/ping", () => Results.Ok("pong"));

// Fallback для SPA — ДОЛЖЕН идти ПОСЛЕДНИМ из маршрутов
app.MapFallbackToFile("index.html");
app.Run();
// --- End of Program.cs ---