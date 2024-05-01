using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
                .Where(e => e.MessageId == 2)
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


        public async Task<List<User>> GetWorkersTodayAsync()
        {
            var readerIds = new[] {87,141,124 };
            var users = await _dbContext.Events
                .AsNoTracking()
                .Where(e => e.DateTime > DateTime.Today && e.MessageId == 2)
                .Where(e=>readerIds.Contains(e.ReaderId))
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
                .Include(e=>e.User)
                .Include(e=>e.Reader)
                .OrderByDescending(e => e.DateTime)
                .FirstOrDefaultAsync(CancellationToken.None);
            
            var events = await _dbContext.Events
                .AsNoTracking()
                .Include(e=>e.Reader)
                .OrderBy(e => e.DateTime)
                //.Where(e => e.UserId == userId)
                //.Select(e => new Event() {User = e.User, Reader = e.Reader, DateTime = e.DateTime, Message = e.Message  })
                .Select(e => new Event() { User = e.User, Reader = e.Reader, DateTime = e.DateTime})

                .LastOrDefaultAsync(e => e.User.Id == userId, CancellationToken.None);
            //return events.DateTime;
            return events;
        }

        public async Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, List<int> inputReader, List<int> outputReader, int messageId = 2)
        {



            //var t1 = await _dbContext.Events
            //    .Where(e => e.User.Id == 5)
            //    .GroupBy(e => e.DateTime.Date)
            //    .Select(e => new { Date = e.Key, Count = e.Count() })
            //    .OrderByDescending(e => e.Date)
            //    .ToListAsync(CancellationToken.None);

            //var user1 = await _dbContext.Users
            //    .Include(e => e.Events!.Where(e => e.DateTime >= startDate && e.DateTime <= endDate))
            //    .ToListAsync(CancellationToken.None);
            //    //.FirstOrDefaultAsync(u => u.Id == 3390);
            //    foreach (var u in user1)
            //    {
            //        Console.WriteLine(u.Name);
            //        foreach (var dt in u.GetWorkingDaysList().Order())
            //        {
            //            Console.WriteLine(dt);
            //        }
            //    }
            //var workingDaysList = user1.GetWorkingDaysList();
            //await GetLastUseKey(5);

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            var usersList = await _dbContext.Users
                .AsNoTracking()
                .Include(e => e.Group)
                .Include(u => u.Events!
                    .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                    .Where(e => e.MessageId == messageId)
                    .Where(e => inputReader.Contains(e.ReaderId) || outputReader.Contains(e.ReaderId))
                    .OrderBy(e => e.DateTime)).ThenInclude(e => e.Reader)
                .ToListAsync(CancellationToken.None);

            stopwatch.Stop();
            //смотрим сколько миллисекунд было затрачено на выполнение
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);



            var report = new Report
            {
                Start = startDate,
                End = endDate
            };


            foreach (var user in usersList)
            {

                var worker = new Worker();
                worker.Group = new Group
                {
                    Name = user.Group.Name
                };

                worker.FullName = user.FullName;
                worker.Name = user.Name;

                var pre = new Event();
                foreach (var evt in user.Events)
                {
                    if (inputReader.Exists(x => x == pre.ReaderId) && outputReader.Exists(x => x == evt.ReaderId))
                    {
                        var v = new WorkTime(pre.DateTime, evt.DateTime)
                        {
                            FirstReader = pre.Reader.Name,
                            LastReader = evt.Reader.Name
                        };
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
        /// <summary>
        /// Возвращает с  список сотрудников с событиями по всем устройствам за определенный период 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<User>> GetUserListWithEventsByDateRange(DateTime startDate, DateTime endDate)
        {
            var userList = await _dbContext.Events
                .AsNoTracking()
                .Include(e=>e.User)
                .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                .GroupBy(e=>e.User).Select(e=>e.Key)
                .OrderBy(e=>e.Name)
                .ToListAsync(CancellationToken.None);

            return userList;


            //var usersList = await _dbContext.Users
            //        .AsNoTracking()
            //        .Include(t => t.Events)!.ThenInclude(e => e.Message)
            //        .Include(t => t.Events)!.ThenInclude(e => e.Reader)
            //        .Include(e => e.Group)

            //    .Select(e => new User()
            //    {
            //        Id = e.Id,
            //        Name = e.Name,
            //        FullName = e.FullName,
            //        Group = e.Group,
            //        Events = e.Events
            //            .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
            //            .ToList()
            //    })
            //        .ToListAsync(CancellationToken.None);
            //var u = usersList.Where(u=>u.Id == 3963).ToList();
            //return usersList;
        }


        /// <summary>
        /// Текущий список сотрудников на производстве
        /// </summary>
        /// <returns></returns>
        public async Task CurrentWorkerList()
        {
            //// Устройства входа
            //var inputReader = new List<int> { 141, 87};
            //var usersList = await _dbContext.Users
            //    .AsNoTracking()
            //    //.Include(e => e.Group)
            //    .Include(u => u.Events!
            //        //.Where(e=> inputReader.Contains(e.ReaderId))
            //        .Where(e => e.DateTime >= DateTime.Parse("2024-02-01 06:00:00.00"))
            //        .OrderBy(e => e.DateTime))
            //    .ToListAsync(CancellationToken.None);
            //foreach (var user in usersList)
            //{
            //    Console.WriteLine($"{user.Name}");
            //}


            var outputReader = new List<int> { 142, 88 };

            //var users = await _dbContext.Users
            //    .Include(e => e.Events!.Where(e => e.DateTime >= DateTime.Parse("2024-02-01 06:00:00.00") && !outputReader.Contains(e.ReaderId)))
            //    .ToListAsync(CancellationToken.None);


            //var res = users.Where(e => e.Events!.Count > 0).OrderBy(e=>e.Name).ToList();


            
        }

        public async Task<List<Event>> TrackingByUserIdAndDateAsync(int userId, DateTime dateTime)
        {
            var tackPoints = await _dbContext.Events
                .AsNoTracking()
                .Include(e=>e.User)
                .Include(e=>e.Reader)
                .Where(e=>e.UserId == userId)
                .Where(e=>e.MessageId == 2)
                .Where(e=>e.DateTime >= dateTime && e.DateTime <= dateTime.AddDays(1))
                .ToListAsync(CancellationToken.None);
            return tackPoints;

        }
    }
}
