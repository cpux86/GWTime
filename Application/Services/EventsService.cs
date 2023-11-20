using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Diagnostics;

namespace Application.Services
{
    //public class EventsService : IEventsService
    //{
    //    private readonly IEventsDbContext _dbContext;

    //    public EventsService(IEventsDbContext dbContext) => _dbContext = dbContext;



    //    public async Task<List<Event>> GetEventsByUserId(int inputReader, int outputReader, int userId)
    //    {

    //        var startDt = DateTime.Parse("23.10.2023 00:00:00");
    //        var endDt = DateTime.Parse("23.10.2023 23:59:59");

    //        var t = new DateTime(2023, 10, 23, 0,0,0);
    //        var today = DateTime.Now;
            
    //        var te =ISOWeek.GetWeekOfYear(DateTime.Now);
    //        var dateTime = ISOWeek.GetYearStart(DateTime.Now.Year).AddDays(43*7).AddSeconds((3600*24)-1);
            

    //        var workTimes = await _dbContext
    //            .Events
    //            .AsNoTracking()
    //            .Where(e => e.dateTime >= startDt && e.dateTime <= endDt)
    //            .Where(e => e.userId > 0)
    //            .Where(e => e.readerId == inputReader || e.readerId == outputReader)
    //            .Where(e=>e.eventCode == 2)
    //            .ToListAsync(CancellationToken.None);
    //        var min = workTimes.Where(e=>e.userId == 5).Min(e => e.dateTime);
    //        var max = workTimes.Where(e => e.userId == 5).Max(e => e.dateTime);
    //        var res = max - min;

    //        return workTimes;

    //    }

    //    public async Task<List<User>> GetUserListAsync()
    //    {

    //        var users = await _dbContext.Events
    //            .AsNoTracking()
    //            //.Where(p => EF.Functions.Like(p.fio, "%Каськов%")
    //            .Select(u => new User{ FullName = u.fio, Group = u.group})
    //            .ToListAsync(CancellationToken.None);
    //        return users;
    //    }

    //    public async Task<List<User>> GetUserByNameAsync(string name)
    //    {
    //        var test = await _dbContext
    //            .Events
    //            .AsNoTracking()
    //            .Where(p => EF.Functions.Like(p.fio, $"{name}%"))
    //            .Select(u => new User { Id = u.Id, FullName = u.fio, Group = u.group })
    //            .ToListAsync(CancellationToken.None);
    //        return test;
    //    }
    //}
}
