using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IEventsDbContext _dbContext;
        public ReportService(IEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        // TimeSheet - учет рабочего время


        /// <summary>
        /// Первое и последнее использование ключа
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="inputReader"></param>
        /// <returns></returns>
        //public async Task<List<Event>> GetFirstAndLastUseKey(int userId, DateTime startDate, DateTime endDate)
        //{
        //    var events = await _dbContext.Events
        //        .Where(e => e.dateTime >= startDate && e.dateTime <= endDate)
        //        .Where(e=>e.eventCode == 2)
        //        .Where(e => e.userId > userId)
        //        .AsNoTracking()
        //        .ToListAsync(CancellationToken.None);
        //    return events;

        //    //var sw = new Stopwatch();
        //    //sw.Start();
        //    //var start = DateTime.Parse("2023-10-16 00:00:00.0000000");
        //    //var end = DateTime.Now;
        //    //var events = await _dbContext.Events
        //    //    .AsNoTracking()
        //    //    //.Where(e => e.eventCode == 2)
        //    //    //.Where(e => e.dateTime >= start && e.dateTime <= end)

        //    //    //.Where(e => e.userId == 2954)
        //    //    //.Where(e => e.readerId == 79 || e.readerId == 80)
        //    //    //.Select(e => new { e.dateTime, e.userId, e.name })
        //    //    //.OrderByDescending(e=>e.userId).ThenByDescending(e=>e.dateTime)
        //    //    //.OrderBy(e => e.userId).ThenBy(e => e.dateTime)
        //    //    .ToListAsync(CancellationToken.None);
        //    //var t = sw.ElapsedMilliseconds;
        //    //return new List<Event>();
        //}

        //public async Task<List<string>> GetReportByReaders(DateTime startDate, DateTime endDate, int inputReader, int outputReader)
        //{
        //    var events = await _dbContext.Events
        //        .AsNoTracking()
        //        .Where(e => e.eventCode == 2)
        //        .Where(e => e.readerId == inputReader || e.readerId == outputReader)
        //        //.Where(e => e.dateTime >= start && e.dateTime <= end)
        //        //.Where(e=>e.userId == 5)
        //        //.Where(e => e.readerId == 79 || e.readerId == 80)
        //        //.Select(e=> new {e.readerId, e.dateTime, e.userId, e.name})
        //        //.OrderByDescending(e=>e.userId).ThenByDescending(e=>e.dateTime)
        //        .OrderBy(e => e.userId).ThenBy(e => e.dateTime)
        //        .ToListAsync(CancellationToken.None);
        //    //var input = 80;
        //    //var output = 79;

        //    var list = new List<string>();

        //    var preEvent = new Event();
        //    foreach (var current in events)
        //    {

        //        if (preEvent.readerId == inputReader && current.readerId == outputReader && preEvent.userId == current.userId)
        //        {
        //            list.Add(
        //                $"Name {current.name} Прибыл: {preEvent.dateTime} Убыл: {current.dateTime} Итого: {current.dateTime.Subtract(preEvent.dateTime)}");
        //            //System.Diagnostics.Debug.WriteLine($"Name {current.name} Прибыл: {preEvent.dateTime} Убыл: {current.dateTime} Итого: {current.dateTime.Subtract(preEvent.dateTime)}");
        //        }
        //        preEvent = current;
        //    }
        //    return list;
        //}


        public async Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, List<int> inputReader, List<int> outputReader)
        {
            var usersList = await _dbContext.Users
                .AsNoTracking()
                .Include(e => e.UserGroup)
                .Include(u => u.Events!
                    .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                    .Where(e => inputReader.Contains(e.ReaderId) || outputReader.Contains(e.ReaderId))
                    .OrderBy(e => e.DateTime)).ThenInclude(e => e.Reader)
                .ToListAsync(CancellationToken.None);

            var report = new Report();
            report.Start = startDate;
            report.End = endDate;
            

            foreach (var user in usersList)
            {
                //if (user.Events != null && !user.Events.Any()) continue;

                var worker = new Worker();
                worker.Group = new Group
                {
                    Name = user.UserGroup.Name
                };

                worker.FullName = user.FullName;
                worker.Name = user.Name;
                //worker.group = user.UserGroup.Name;
                
                
                var pre = new Event();
                foreach (var evt in user.Events)
                {
                    var v = new WorkTime();
                    if (inputReader.Exists(x => x == pre.ReaderId) && outputReader.Exists(x => x == evt.ReaderId))
                    {
                        v.EntryTime = pre.DateTime;
                        v.ExitTime = evt.DateTime;
                        v.FirstReader = pre.Reader.Name;
                        v.LastReader = evt.Reader.Name;
                        worker.WorkTimes.Add(v);
                    }

                    pre = evt;
                }

                report.Workers.Add(worker);

            }

            var users = report.Workers.OrderBy(e => e.FullName)
                .ThenBy(e => e.Group)
                .ToList<Worker>();
            var groups = users.GroupBy(e => e.Group!.Name)
                .Select(e => new Group { Name = e.Key, Workers = e.ToList<Worker>() })
                .OrderBy(e => e.Name)
                .ToList();

            report.Groups = groups;
            return report;

        }
    }
}
