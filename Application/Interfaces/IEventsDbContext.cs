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
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Reader> Readers { get; set; }
        /// <summary>
        /// Возвращает даты рабочих дней
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<List<DateTime>> GetWorkingDaysByUserId(int userId, DateTime start, DateTime end);
        //public Task<List<DateOnly>> GetWorkingDaysByUserId(int userId, DateTime start, DateTime end);
    }
}
