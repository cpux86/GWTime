using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            //services.AddScoped<IEventsDbContext, EventsDbContext>();
            //services.AddSingleton<EventsDbContext>();
            services.AddSingleton<IEventsDbContext, EventsDbContext>();
            services.AddDbContextPool<EventsDbContext>(options =>
            {
                //options.UseSqlite(@"DataSource=C:\CSharp\GWTime\DB\Event.db");

                //options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=tc;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

                //options.UseSqlServer(
                //    @"Data Source=WIN-8EEFM1GR5NJ\SQLEXPRESS; Database=tc; Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False",
                //    o=> o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                
                //options.LogTo(message => System.Diagnostics.Debug.WriteLine(message));

                //options.UseSqlServer("Server=176.57.78.32; Database=GWTime; User ID=sa; Password=1AC290066f_;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
                //options.UseSqlServer("Server=10.65.68.252; Database=GWTime_test; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
                options.UseSqlServer("Server=10.65.68.252; Database=GWTime_test1; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            });
        }
    }
}