using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using WorkingCalendar.Server.Infrastructure;
using WorkingCalendar.Server.Infrastructure.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<IWorkingCalendarRepository, WorkingCalendarRepository > ();
        builder.Services.AddTransient<IWorkingCalendarService, WorkingCalendarService>();
        // Add services to the container.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                              builder =>
                              {
                                  builder.WithOrigins("http://231977.fornex.cloud",
                                                      "https://231977.fornex.cloud",
                                                      "http://localhost",
                                                      "https://localhost",
                                                      "http://localhost:3000",
                                                      "https://localhost:3000");
                              });
        });
        builder.Services.AddControllers().AddXmlDataContractSerializerFormatters();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
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
        //app.UseForwardedHeaders();
        app.UseDefaultFiles();
       
        app.UseHealthChecks("/hc");
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("Development");
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            //app.UseHsts();
        }
        //app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCors(MyAllowSpecificOrigins);
        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}