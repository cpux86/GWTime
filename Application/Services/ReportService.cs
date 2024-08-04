using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.DTOs;
using Application.Features.Tacking;
using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Group = Application.DTOs.Group;


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
        public async Task<List<Event>> GetFirstAndLastUseKey(int userId, DateTime startDate, DateTime endDate)
        {
            var sw = new Stopwatch();
            sw.Start();
            var start = DateTime.Parse("2023-10-16 00:00:00.0000000");
            var end = DateTime.Now;
            var events = await _dbContext.Events
                .AsNoTracking()
                .Where(e => e.Code == 2)
                .Where(e => e.DateTime >= start && e.DateTime <= end)

                .Where(e => e.UserId == 2954)
                .Where(e => e.ReaderId == 79 || e.UserId == 80)
                .Select(e => new { e.DateTime, e.UserId, e.User.Name })
                .OrderByDescending(e => e.UserId).ThenByDescending(e => e.DateTime)
                .OrderBy(e => e.UserId).ThenBy(e => e.DateTime)
                .ToListAsync(CancellationToken.None);
            var t = sw.ElapsedMilliseconds;
            return new List<Event>();
        }

        /// <summary>
        /// Список сотрудников за сегодня
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetWorkersTodayAsync()
        {
            // учитывать время до начала рабочего дня не более


            var dayShift = DateTime.Today.AddHours(8);
            var nightShift = dayShift.AddHours(12);
            var startWorkDay = DateTime.Today.AddHours(7);

            var readerIds = new[] { 141, 87,/* 142, 88*/ };
            var users = await _dbContext.Events
                .AsNoTracking()
                //.Where(e => e.DateTime > DateTime.Today && e.Code == 2)
                .Where(e => e.DateTime > startWorkDay && e.DateTime <= startWorkDay.AddHours(4) && e.Code == 2)
                .Where(e=>readerIds.Contains(e.ReaderId))
                .Include(e => e.User).ThenInclude(u => u.Group)
                .Select(e => e.User).OrderBy(e => e.Group.Id).ThenBy(e => e.Name)

                .ToListAsync(CancellationToken.None);

            var t = users.DistinctBy(e => e.Name).ToList();
            return t;
        }

        public async Task<List<User>> GetNightShiftAsync()
        {
            // учитывать время до начала рабочего дня не более


            var dayShift = DateTime.Today.AddHours(8);
            var nightShift = dayShift.AddHours(12-2);
            var startWorkDay = DateTime.Today.AddHours(7);

            var readerIds = new[] { 141, 87,/* 142, 88*/ };
            var users = await _dbContext.Events
                .AsNoTracking()
                //.Where(e => e.DateTime > DateTime.Today && e.Code == 2)
                .Where(e => e.DateTime > nightShift && e.Code == 2)
                .Where(e => readerIds.Contains(e.ReaderId))
                .Include(e => e.User).ThenInclude(u => u.Group)
                .Select(e => e.User).OrderBy(e => e.Group.Id).ThenBy(e => e.Name)

                .ToListAsync(CancellationToken.None);

            var t = users.DistinctBy(e => e.Name).ToList();
            return t;
        }


        // последнее 
        public async Task<Event> GetLastUseKey(int userId)
        {

            var t = await _dbContext.Events
                .AsNoTracking()
                .Where(e => e.User.Id == userId)
                .Include(e => e.User)
                .Include(e => e.Reader)
                .OrderByDescending(e => e.DateTime)
                .FirstOrDefaultAsync(CancellationToken.None);
                //.FirstOrDefaultAsync(CancellationToken.None) ?? throw new Exception();

            return t;
        }

        public async Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, List<int> inputReader, List<int> outputReader, int messageId = 2)
        {
            //startDate = DateTime.Parse("2024-07-16 06:00:00.00");
            //startDate = DateTime.Parse("2024-07-13 06:00:00.00");
            //endDate = DateTime.Parse("2024-07-10 10:00:00.00");
            var usersList = await _dbContext.Users
                .AsNoTracking()
                .Include(e => e.Group)
                .Include(u => u.Events!
                    .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                    .Where(e => e.Code == messageId)
                    //.Where(e=>e.UserId == 3985)
                    //.Where(e=>e.UserId == 4641)
                    //.Where(e => e.UserId == 4552)
                    //.Where(e => e.UserId == 4469)
                    //.Where(e => e.UserId == 4198)
                    //.Where(e => e.UserId == 4635)
                    .Where(e => e.UserId == 4649)
                    //.Where(e=>e.ReaderId != 109)
                    .Where(e => inputReader.Contains(e.ReaderId) || outputReader.Contains(e.ReaderId))
                    .OrderBy(e => e.DateTime))
                .ThenInclude(e => e.Reader)
                .ToListAsync(CancellationToken.None);

            var report = new Report
            {
                Start = startDate,
                End = endDate
            };



            foreach (var user in usersList)
            {
                var worker = new Worker
                {
                    FullName = user.FullName,
                    Name = user.Name,
                    Group = new Group
                    {
                        Name = user.Group.Name
                    }
                };

                var e1 = new Event();
                TimeSpan ts = TimeSpan.Zero;
                Event evtIn = null;
                Event evtOut = null;


                    foreach (var e2 in user.Events)
                    {
                        if (evtIn == null)
                        {
                            e1 = e2;
                            evtIn = e2;
                            continue;
                        }


                        var tmpTs = e2.DateTime - e1.DateTime;
                        


                        if (outputReader.Contains(e1.ReaderId) && tmpTs.TotalHours > 6)
                        {
                            //Console.WriteLine(1);
                            if (evtOut != null)
                            {
                                var v = new WorkShift(evtIn.DateTime, evtOut.DateTime)
                                {
                                    FirstReader = evtIn.Reader.Name,
                                    LastReader = evtOut.Reader.Name
                                };
                                Console.WriteLine($"{worker.Name} - {v.EntryTime} - {v.ExitTime} итого: {v.Tot}");
                                worker.WorkTimes.Add(v);
                                worker.ContractInfo = "Т/Д";
                                evtOut = null;
                            }


                            //Console.WriteLine($"{e1.User.Name} Начало новой смены - {e2.DateTime}");
                            //Console.WriteLine($"{e2.User.Name} - |{evtIn.DateTime} {e1.DateTime}| {ts}");
                            e1 = e2;

                            ts = TimeSpan.Zero;
                            evtIn = e2;
                            continue;
                        }

                        evtOut = e2;
                        ts += tmpTs;
                        e1 = e2;
                    }
                report.Workers.Add(worker);
            }




            //usersList.ForEach(user =>
            //{
            //    var worker = new Worker
            //    {
            //        FullName = user.FullName,
            //        Name = user.Name,
            //        Group = new Group
            //        {
            //            Name = user.Group.Name
            //        }
            //    };

            //    var e1 = new Event();
            //    TimeSpan ts = TimeSpan.Zero;
            //    Event evtIn = null;
            //    Event evtOut = null;




            //    foreach (var e2 in user.Events)
            //    {
            //        var t = user.Events;
            //        if (evtIn == null)
            //        {
            //            evtIn = e2;
            //            e1 = e2;
            //            return;
            //        }


            //        var tmpTs = e2.DateTime - e1.DateTime;
            //        evtOut = e2;

            //        if (outputReader.Contains(e1.ReaderId) && tmpTs.Hours > 6)
            //        {
            //            Console.WriteLine($"{e1.User.Name} Начало новой смены - {e2.DateTime}");
            //            //Console.WriteLine($"{e2.User.Name} - |{evtIn.DateTime} {e1.DateTime}| {ts}");
            //            e1 = e2;

            //            ts = TimeSpan.Zero;
            //            evtIn = e2;
            //            return;
            //        }
            //        ts += tmpTs;
            //    }

            //    //user.Events?.ForEach(e2 =>
            //    //{
            //    //    var t = user.Events;
            //    //    if (evtIn == null)
            //    //    {
            //    //        evtIn = e2;
            //    //        e1 = e2;
            //    //        return;
            //    //    }


            //    //    var tmpTs = e2.DateTime - e1.DateTime;
            //    //    evtOut = e2;

            //    //    if (outputReader.Contains(e1.ReaderId) && tmpTs.Hours > 6)
            //    //    {
            //    //        Console.WriteLine($"{e1.User.Name} Начало новой смены - {e2.DateTime}");
            //    //        //Console.WriteLine($"{e2.User.Name} - |{evtIn.DateTime} {e1.DateTime}| {ts}");
            //    //        e1 = e2;

            //    //        ts = TimeSpan.Zero;
            //    //        evtIn = e2;
            //    //        return;
            //    //    }
            //    //    ts += tmpTs;

            //    //    //Console.WriteLine($"{e2.User.Name} - |{evtIn.DateTime} {e1.DateTime}| {ts}");

            //    //    //if (inputReader.Exists(x => x == e1.ReaderId) && outputReader.Exists(x => x == e2.ReaderId))
            //    //    //{

            //    //    //    var v = new WorkShift(e1.DateTime, e2.DateTime)
            //    //    //    {
            //    //    //        FirstReader = e1.Reader.Name,
            //    //    //        LastReader = e2.Reader.Name
            //    //    //    };
            //    //    //    if (v.Total < TimeSpan.FromMinutes(5))
            //    //    //    {
            //    //    //        //Console.WriteLine($"ID {prev.Id} USR ID {evt.UserId} {evt.User.Name} {prev.DateTime} - {evt.DateTime} {v.Tot}");
            //    //    //    }

            //    //    //    worker.WorkTimes.Add(v);
            //    //    //    worker.ContractInfo = "Т/Д";
            //    //    //}
            //    //    //e1 = e2;

            //    //});

            //    report.Workers.Add(worker);
            //});











            //usersList.AsParallel().ForAll(user =>
            //{
            //    var worker = new Worker
            //    {
            //        FullName = user.FullName,
            //        Name = user.Name,
            //        Group = new Group
            //        {
            //            Name = user.Group.Name
            //        }
            //    };

            //    var e1 = new Event();
            //    TimeSpan ts = TimeSpan.Zero;

            //    user.Events?.ForEach(e =>
            //    {
            //        if (e1.ReaderId > 0)
            //        {

            //            var wt1 = e.DateTime - e1.DateTime;
            //            if (e1.ReaderId == 106 && wt1.Hours > 8)
            //            {
            //                Console.WriteLine(ts);
            //                ts = TimeSpan.Zero;

            //                return;
            //            }
            //            ts += wt1;
            //            //Console.WriteLine(ts);
            //        }


            //        //if (inputReader.Exists(x => x == e1.ReaderId) && outputReader.Exists(x => x == e.ReaderId))
            //        //{

            //        //    var v = new WorkShift(e1.DateTime, e.DateTime)
            //        //    {
            //        //        FirstReader = e1.Reader.Name,
            //        //        LastReader = e.Reader.Name
            //        //    };
            //        //    if (v.Total < TimeSpan.FromMinutes(5))
            //        //    {
            //        //        //Console.WriteLine($"ID {prev.Id} USR ID {evt.UserId} {evt.User.Name} {prev.DateTime} - {evt.DateTime} {v.Tot}");
            //        //    }

            //        //    worker.WorkTimes.Add(v);
            //        //    worker.ContractInfo = "Т/Д";
            //        //}
            //        e1 = e;
            //    });
            //    //Console.WriteLine(ts);
            //    report.Workers.Add(worker);
            //});



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
        /// <summary>
        /// Возвращает список сотрудников прошедших регистрацию в системе за определенный период 
        /// </summary>
        /// <param name="startDate">начало периода</param>
        /// <param name="endDate">окончание</param>
        /// <returns></returns>
        public async Task<List<User>> GetUsersAsync(DateTime startDate, DateTime endDate)
        {
            var users = await _dbContext.Events
                .AsNoTracking()
                .Include(e=>e.User)
                .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                .GroupBy(e=>e.User).Select(e=>e.Key)
                .OrderBy(e=>e.Name)
                .ToListAsync(CancellationToken.None);

            return users;
        }

        /// <summary>
        /// Возвращает рабочие дни месяца 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<DateTime>> GetWorkingDaysByUserId(int userId, DateTime date)
        {
            var startdt = new DateTime(date.Year, date.Month, 1);

            var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            var enddt = startdt.AddDays(daysInMonth);
            return await _dbContext.GetWorkingDaysByUserId(userId, startdt, enddt);
        }

        public async Task<List<Event>> TrackingByUserIdAndDateAsync(int userId, DateTime dateTime)
        {
            var tackPoints = await _dbContext.Events
                .AsNoTracking()
                .Include(e=>e.User)
                .Include(e=>e.Reader)
                .Where(e=>e.UserId == userId)
                .Where(e=>e.Code == 2)
                .Where(e=>e.DateTime >= dateTime && e.DateTime <= dateTime.AddDays(1))
                .ToListAsync(CancellationToken.None);
            return tackPoints;

        }
    }
}
