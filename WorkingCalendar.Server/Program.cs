using System.Reflection;

using Microsoft.OpenApi.Models;

using WorkingCalendar.Server.Infrastructure;
using WorkingCalendar.Server.Infrastructure.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<IWorkingCalendarRepository, WorkingCalendarRepository > ();
        builder.Services.AddTransient<IWorkingCalendarService, WorkingCalendarService>();
        // Add services to the container.

        builder.Services.AddControllers().AddXmlDataContractSerializerFormatters();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            //var basePath = AppContext.BaseDirectory;

            //var xmlPath = Path.Combine(basePath, "ShopAPI.xml");
            //options.IncludeXmlComments(xmlPath);

            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}