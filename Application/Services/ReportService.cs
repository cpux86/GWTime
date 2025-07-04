﻿using System;
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


        public async Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, List<int> entryReaderIds, List<int> exitReaderIds, int messageId = 2)
        {

            var usersList = await _dbContext.Users
                .AsNoTracking()
                .Include(e => e.Group)
                .Include(u => u.Events!
                    .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                    .Where(e => e.Code == messageId)
                    //.Where(e => e.UserId == 491)
                    .Where(e => entryReaderIds.Contains(e.ReaderId) || exitReaderIds.Contains(e.ReaderId))
                    .OrderBy(e => e.DateTime)).ThenInclude(e => e.Reader)
                .ToListAsync(CancellationToken.None);


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


                var prev = new Event();
                foreach (var evt in user.Events)
                {


                    //if (prev.ReaderId == evt.ReaderId)
                    //{
                    //    continue;
                    //}

                    if (entryReaderIds.Exists(x => x == prev.ReaderId) && exitReaderIds.Exists(x => x == evt.ReaderId))
                    {

                        var v = new WorkTime(prev.DateTime, evt.DateTime)
                        {
                            FirstReader = prev.Reader.Name,
                            LastReader = evt.Reader.Name
                        };
                                        
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



        //public async Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, List<int> inputReader, List<int> outputReader, int messageId = 2)
        //{
        //    // Используем HashSet для быстрого поиска (O(1) вместо O(n) в List)
        //    var inputReaderSet = new HashSet<int>(inputReader);
        //    var outputReaderSet = new HashSet<int>(outputReader);

        //    var usersList = await _dbContext.Users
        //        .AsNoTracking()
        //        .Where(u => u.Events.Any(e =>
        //            e.DateTime >= startDate && e.DateTime <= endDate &&
        //            e.Code == messageId &&
        //            (inputReaderSet.Contains(e.ReaderId) || outputReaderSet.Contains(e.ReaderId))))
        //        .Select(u => new
        //        {
        //            u.FullName,
        //            u.Name,
        //            GroupName = u.Group.Name,
        //            Events = u.Events
        //                .Where(e => e.DateTime >= startDate && e.DateTime <= endDate &&
        //                            e.Code == messageId &&
        //                            (inputReaderSet.Contains(e.ReaderId) || outputReaderSet.Contains(e.ReaderId)))
        //                .OrderBy(e => e.DateTime)
        //                .Select(e => new
        //                {
        //                    e.Id,
        //                    e.DateTime,
        //                    e.ReaderId,
        //                    ReaderName = e.Reader.Name,
        //                    e.UserId
        //                })
        //                .ToList()
        //        })
        //        .ToListAsync();

        //    var report = new Report
        //    {
        //        Start = startDate,
        //        End = endDate
        //    };

        //    foreach (var user in usersList)
        //    {
        //        if (user.Events.Count == 0) continue;

        //        var worker = new Worker
        //        {
        //            FullName = user.FullName,
        //            Name = user.Name,
        //            Group = new Group { Name = user.GroupName }
        //        };

        //        var prev = user.Events.First();

        //        foreach (var evt in user.Events.Skip(1))
        //        {
        //            if (prev.ReaderId == evt.ReaderId) continue;

        //            if (inputReaderSet.Contains(prev.ReaderId) && outputReaderSet.Contains(evt.ReaderId))
        //            {
        //                var workTime = new WorkTime(prev.DateTime, evt.DateTime)
        //                {
        //                    FirstReader = prev.ReaderName,
        //                    LastReader = evt.ReaderName
        //                };

        //                if (workTime.Total < TimeSpan.FromMinutes(5))
        //                {
        //                    Console.WriteLine($"ID {prev.Id} USR ID {evt.UserId} {user.Name} {prev.DateTime} - {evt.DateTime} {workTime.Total}");
        //                }

        //                worker.WorkTimes.Add(workTime);
        //                worker.ContractInfo = "Т/Д";
        //            }
        //            prev = evt;
        //        }

        //        if (worker.WorkTimes.Count > 0)
        //        {
        //            report.Workers.Add(worker);
        //        }
        //    }

        //    report.Workers = report.Workers
        //        .OrderBy(w => w.FullName)
        //        .ThenBy(w => w.Group.Name)
        //        .ToList();

        //    report.Groups = report.Workers
        //        .GroupBy(w => w.Group.Name)
        //        .Select(g => new Group { Name = g.Key, Workers = g.ToList() })
        //        .OrderBy(g => g.Name)
        //        .ToList();

        //    return report;
        //}



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
                .Include(e => e.User)
                .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                .GroupBy(e => e.User).Select(e => e.Key)
                .OrderBy(e => e.Name)
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
