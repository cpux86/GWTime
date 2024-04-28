namespace GateLogger.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            //services.AddScoped<IEventsDbContext, EventsDbContext>();
            //services.AddSingleton<EventsDbContext>();
            //services.AddSingleton<IEventsDbContext, EventsDbContext>();
            services.AddDbContextPool<EventsDbContext>(options =>
            {


                //options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=tc;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

                options.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
                //options.UseSqlite(@"DataSource=Event_temp.db");
                //options.UseSqlServer("Server=10.65.68.252; Database=GWTime_test; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            });
        }
    }
}