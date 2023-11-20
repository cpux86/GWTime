using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEventsDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
