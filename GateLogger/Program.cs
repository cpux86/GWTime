using GateLogger;
using GateLogger.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {

        services.AddDbContext<EventsDbContext>(opt =>
            opt.UseSqlServer("Server=10.65.68.252; Database=GWTime_test; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"),
            ServiceLifetime.Singleton);



        //options.UseSqlServer("Server=10.65.68.252; Database=GWTime; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        services.AddHostedService<Worker>();
        services.AddPersistence();
        services.AddMemoryCache();
       
    })
    .Build();

await host.RunAsync();



