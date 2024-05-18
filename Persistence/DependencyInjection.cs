using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Persistence
{
    public static class DependencyInjection
    {
        //private IConfiguration _configuration;
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddSingleton<IEventsDbContext, EventsDbContext>();
            //services.AddScoped<IEventsDbContext, EventsDbContext>();
            //services.AddDbContext<EventsDbContext>(options =>
            //{
            //    options.UseSqlServer("Server=10.65.68.252; Database=GWTime_test2; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            //    options.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
            //});


            services.AddDbContextPool<EventsDbContext>(options =>
            {

                //var t = configuration.GetSection("ConnectionStrings:GWT0905");
                var connectionString = configuration.GetConnectionString("GateLogger");

                options.UseSqlServer(connectionString);

                //options.UseSqlite(@"DataSource=C:\CSharp\GWTime\DB\Event.db");
                //options.UseSqlServer(System.Environment.GetEnvironmentVariable("GWT0905"));

                //options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=tc;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

                //options.UseSqlServer(
                //    @"Data Source=WIN-8EEFM1GR5NJ\SQLEXPRESS; Database=tc; Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False",
                //    o=> o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));




                //options.UseSqlServer("Server=176.57.78.32; Database=GWTime; User ID=sa; Password=1AC290066f_;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
                //options.UseSqlServer("Server=10.65.68.252; Database=GWTime_test; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
                //options.UseSqlServer("Server=10.65.68.252; Database=GWTime_test2.1; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
                //options.LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Debug);
            });
        }
    }
}