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

            //inputReader.AddRange(outputReader);

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            var usersList = await _dbContext.Users
                .AsNoTracking()
                .Include(e => e.Group)
                .Include(u => u.Events!
                    .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                    .Where(e => e.Code == messageId)
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

            var ids = usersList.Where(e=>e.Events.Count > 0).Select(e => e.Id).ToArray();

            foreach (var user in usersList)
            {

                var worker = new Worker();
                worker.Group = new Group
                {
                    Name = user.Group.Name
                };

                worker.FullName = user.FullName;
                worker.Name = user.Name;

                var prev = new Event();
                foreach (var evt in user.Events)
                {
                    if (inputReader.Exists(x => x == prev.ReaderId) && outputReader.Exists(x => x == evt.ReaderId))
                    {

                        var v = new WorkTime(prev.DateTime, evt.DateTime)
                        {
                            FirstReader = prev.Reader.Name,
                            LastReader = evt.Reader.Name
                        };
                        if (v.Total < TimeSpan.FromMinutes(5))
                        {
                            Console.WriteLine($"ID {prev.Id} USR ID {evt.UserId} {evt.User.Name} {prev.DateTime} - {evt.DateTime} {v.Tot}");
                        }

                        worker.WorkTimes.Add(v);
                        worker.ContractInfo = "Т/Д";
                    }
                    prev = evt;
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
