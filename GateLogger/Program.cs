using System.Globalization;
using GateLogger;
using GateLogger.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

//var host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices(services =>
//    {
//        //services.AddDbContext<EventsDbContext>(opt =>
//        //        opt.UseSqlServer(
//        //            "Server=10.65.68.252; Database=GWTime_test; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"),
//        //    ServiceLifetime.Singleton);

//        //options.UseSqlServer("Server=10.65.68.252; Database=GWTime; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
//        services.AddHostedService<Worker>();
//        //services.AddWindowsService<Worker>();
//        services.AddPersistence();
//        services.AddMemoryCache();

//    })
//    .Build();

CultureInfo.CurrentCulture = new CultureInfo("ru-RU");


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Gate Logger";
});

LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

builder.Services.AddPersistence();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();

//await host.RunAsync();



