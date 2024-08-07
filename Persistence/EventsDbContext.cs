﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class EventsDbContext : DbContext, IEventsDbContext
    {
        public EventsDbContext(DbContextOptions options) : base(options) { }

        public EventsDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Reader> Readers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Возвращает даты рабочих дней
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<DateTime>> GetWorkingDaysByUserId(int userId, DateTime startDate, DateTime endDate)
        {


            //var d = await Database.SqlQueryRaw<DateOnly>($"SELECT DISTINCT CONVERT(date, dateTime) AS dt FROM Events" +
            //                                      $" WHERE UserId = @userId and dateTime > @startDt and dateTime < @endDt" +
            //                                      $" ORDER BY dt ASC", new SqlParameter("userId", userId), new SqlParameter("startDt", startDate), new SqlParameter("endDt", endDate))
            //    .ToListAsync(CancellationToken.None);

            return await Database.SqlQueryRaw<DateTime>($"SELECT DISTINCT CONVERT(date, dateTime) AS dt FROM Events" +
                                                         $" WHERE UserId = @userId and dateTime > @startDt and dateTime < @endDt" +
                                                         $" ORDER BY dt ASC", new SqlParameter("userId", userId), new SqlParameter("startDt", startDate), new SqlParameter("endDt", endDate))
                .ToListAsync(CancellationToken.None);

            //return await Database.SqlQueryRaw<DateTime>($"SELECT DISTINCT CONVERT(date, dateTime) AS dt FROM Events" +
            //                                      $" WHERE UserId = @userId and dateTime > @startDt and dateTime < @endDt" +
            //                                      $" ORDER BY dt ASC", new SqlParameter("userId", userId), new SqlParameter("startDt", startDate), new SqlParameter("endDt", endDate))
            //    .ToListAsync(CancellationToken.None);
        }
    }
}
