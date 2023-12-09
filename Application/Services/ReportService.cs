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
            var t = await _dbContext.Users
                .Include(u=>u.UserGroup)
                .ToListAsync(CancellationToken.None);

            var u = await _dbContext.Users
                //.Where(u=>u.FullName == "Каськов Владимир Васильевич")
                //.Where(u=>usr.Contains(u.Id))
                //.Where(u=>u.Group == "Техническая служба")
                .AsNoTracking()
                .Include(u => u.Events!
                    .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                    .Where(e => e.EventCode == 2)
                    .Where(e => inputReader.Contains(e.ReaderId) || outputReader.Contains(e.ReaderId))
                    .OrderBy(e => e.DateTime)).ThenInclude(e => e.Reader)
                
                .ToListAsync(CancellationToken.None);

            var report = new Report();
            report.Start = startDate;
            report.End = endDate;

            foreach (var user in u)
            {
                if (user.Events != null && !user.Events.Any()) { continue; }

                var worker = new Worker();
                worker.FullName = user.FullName;
                worker.Group = user.Group;
                worker.Name = user.Name;
                
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
                        //Console.WriteLine($"{v.FirstReader} - {v.LastReader} {v.Tot}");
                    }

                    pre = evt;
                }


                report.Workers.Add(worker);

            }

            // var list1 = u.Select(user => user.GetWorkList(inputReader, outputReader)).Where(wtList => wtList.Count > 0).Cast<object>().ToList();
            var test = report.Workers.OrderBy(e => e.FullName).ThenBy(e=>e.Group).ToList<Worker>();
            report.Workers = test;
            return report;


            var events = await _dbContext.Events
                .AsNoTracking()
                .Include(e => e.Reader)
                .Include(e => e.User)
                .Where(e => e.EventCode == 2)
                .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                .Where(e=> inputReader.Contains(e.ReaderId) || outputReader.Contains(e.ReaderId))

                .OrderBy(e => e.UserId).ThenBy(e => e.DateTime)
                .ToListAsync(CancellationToken.None);

           
            var list = new List<string>();
            var wt = new List<WorkTime>();

            var preEvent = new Event();
            //foreach (var current in from current in events let exists = inputList.Exists(x => x == preEvent.ReaderId) select current)
            foreach (var current in events)
            {
                //var exists = inputList.Exists(x => x == preEvent.ReaderId);

                //if (preEvent.ReaderId == inputReader && current.ReaderId == outputReader && preEvent.UserId == current.UserId)

                if (inputReader.Exists(x => x == preEvent.ReaderId) && outputReader.Exists(x => x == current.ReaderId) && preEvent.UserId == current.UserId)
                {
                    //var report = new Report
                    //{
                    //    Start = startDate,
                    //    End = endDate
                    //};

                    //var worker = new Worker
                    //{
                    //    FullName = current.User.FullName
                    //};

                    //wt.Add(new WorkTime()
                    //{
                    //    EntryTime = preEvent.DateTime,
                    //    ExitTime = current.DateTime,
                    //    FirstReader = preEvent.Reader.Name,
                    //    LastReader = current.Reader.Name,
                    //    Total = current.DateTime.Subtract(preEvent.DateTime)
                    //});

                    list.Add($"{current.User?.FullName} " +
                             $"{preEvent.Reader.Name}: {preEvent.DateTime}" +
                             $" {current.Reader.Name}: {current.DateTime} " +
                             $"Итого: {(int)current.DateTime.Subtract(preEvent.DateTime).TotalHours:00}:{current.DateTime.Subtract(preEvent.DateTime).Minutes:00}"
                    );
                }

                preEvent = current;
            }

            //return wt;
           // return list;
        }
    }
}
