using Domain;
using Microsoft.EntityFrameworkCore;

namespace GateLogger.Infrastructure
{
    public class EventsDbContext : DbContext     //, IEventsDbContext
    {
        public EventsDbContext(DbContextOptions options) : base(options)
        {
        }
        public EventsDbContext()
        {
            Database.EnsureCreated();

        }
        public DbSet<Event> Events { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reader> Readers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=10.65.68.252; Database=GWT0905; User ID=sa; Password=LaMp368&;Integrated Security=true;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            //optionsBuilder.LogTo(System.Console.WriteLine, LogLevel.Information);
            //.UseSqlServer(System.Environment.GetEnvironmentVariable("ConnectionStrings:GWT0905"));
            //optionsBuilder.UseSqlServer("Server=10.65.68.252; Database=GWTime_test2.12111111; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            //.UseSqlServer("Server=10.65.68.252; Database=GWT0905; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            //optionsBuilder.LogTo(System.Console.WriteLine, LogLevel.Information);
            optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Readers>()
        //        .Property(b => b.CreatedDate)
        //        .ValueGeneratedOnAddOrUpdate();
        //    //.HasDefaultValueSql("getdate()");
        //}


        //public override int SaveChanges()
        //{
        //    var entries = ChangeTracker
        //        .Entries()
        //        .Where(e => e.Entity is BaseEntity && (
        //            e.State == EntityState.Added || e.State == EntityState.Modified));

        //    foreach (var entityEntry in entries)
        //    {
        //        var dt = DateTime.Now;
        //        ((BaseEntity)entityEntry.Entity).UpdatedDate = dt;

        //        if (entityEntry.State == EntityState.Added)
        //        {
        //            ((BaseEntity)entityEntry.Entity).CreatedDate = dt;
        //        }
        //    }

        //    return base.SaveChanges();
        //}

    }
}
