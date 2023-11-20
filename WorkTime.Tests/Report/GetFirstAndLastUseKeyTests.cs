using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Persistence;

namespace WorkTime.Tests.Report
{
    public class GetFirstAndLastUseKeyTests
    {
        private readonly IEventsDbContext _dbContext;

        public GetFirstAndLastUseKeyTests()
        {
            _dbContext = new EventsDbContext(
                new DbContextOptionsBuilder<EventsDbContext>()
                    .UseSqlServer("Server=10.65.68.252; Database=GWTime; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False")
                    .LogTo(message => System.Diagnostics.Debug.WriteLine(message))
                    .Options
            );
        }

        [Theory]
        //[InlineData(5, "2023-10-23 00:00:00.0000000", "2023-10-23 23:59:59.0000000", "12:44:07")]
        //[InlineData(5, "2023-10-24 00:00:00.0000000", "2023-10-24 23:59:59.0000000", "00:00:00")]
        //[InlineData(5, "2023-10-25 00:00:00.0000000", "2023-10-25 23:59:59.0000000", "00:00:00")]
        //[InlineData(5, "2023-10-26 00:00:00.0000000", "2023-10-26 23:59:59.0000000", "00:00:00")]
        //[InlineData(5, "2023-10-27 00:00:00.0000000", "2023-10-27 23:59:59.0000000", "3:05:07")]
        //[InlineData(5, "2023-10-29 00:00:00.0000000", "2023-10-29 23:59:59.0000000", "12:16:03")]
        //[InlineData(5, "2023-10-16 00:00:00.0000000", "2023-11-06 23:59:59.0000000", "12:16:03")]
        //[InlineData(2451, "2023-10-16 00:00:00.0000000", "2023-11-05 23:59:59.0000000", "12:16:03")] // Горбаток
        //[InlineData(3390, "2023-10-16 00:00:00.0000000", "2023-11-05 23:59:59.0000000", "12:16:03")] // Каськов 3469
        //[InlineData(3469, "2023-10-16 00:00:00.0000000", "2023-11-05 23:59:59.0000000", "12:16:03")] // Миндияров 
        [InlineData(4024, "2023-10-16 00:00:00.0000000", "2023-11-05 23:59:59.0000000", "12:16:03")] // Демин 12-12
        public async Task GetFirstAndLastUseKey_ByUserId(int userId, string startDay, string endDay, string result)
        {
            var sw1 = new Stopwatch();
            sw1.Start();
            var offset = TimeSpan.FromHours(1);

            //try
            //{
            //    var start = DateTime.Parse(startDay);

            //    var end = DateTime.Now;
            //    var events = await _dbContext.Events
            //        .AsNoTracking()
            //        .Where(e => e.eventCode == 2)

            //        .Where(e => e.readerId == 131 || e.readerId == 132)

            //        .OrderBy(e => e.userId).ThenBy(e => e.dateTime)
            //        .ToListAsync(CancellationToken.None);

            //    //for (var i = 0; i < t.Count; i++)
            //    //{
            //    //    var sw = new Stopwatch();
            //    //    sw.Start();
            //    //    //var min = t.Min(e => e.dateTime);
            //    //    var min1 = t.First();
            //    //    var min = min1.dateTime;
            //    //    //var test = t.Where(e => e.dateTime.Date < min.AddDays(1))
            //    //    var test = t.LastOrDefault(e => e.dateTime.Date < min.Date.AddDays(1) && e.userId == min1.userId);
            //    //    var max = test.dateTime;

            //    //    var old = t.RemoveAll(e => e.dateTime < min.Date.AddDays(1) && e.userId == min1.userId);


            //    //    var r = max.Subtract(min);
            //    //    //var res = r.ToString("g");
            //    //    sw.Stop();
            //    //    //var time = sw.ElapsedTicks;
            //    //    var time = sw.Elapsed;

            //    //    System.Diagnostics.Debug.WriteLine($"User ID:{min1.name} Дата: {min.ToString("d")} Приход: {min:t} Уход: {max:t} {r:g}     {time}");
            //    //}


            //    var input = 131;
            //    var output = 132;

            //    List<string> list = new List<string>();

            //    var preEvent = new Event();
            //    foreach (var current in events)
            //    {
                   
            //        //if (preEvent.readerId == input && current.readerId == output && preEvent.userId == current.userId)
            //        //{
            //        //    var wt = current.dateTime.Subtract(preEvent.dateTime);
            //        //    var wtt = new TimeSpan(0, 1, 15, 20).ToString();
            //        //    //var t = (int)wt.TotalHours;
            //        //    //var h1 = t.ToString("d2");
            //        //    var dt1 = new DateTime(2023, 01, 01, 01, 01, 01) - new DateTime(2023, 01, 01, 00, 01, 01);
            //        //    var hours = (int)wt.TotalHours;


            //        //    var minutes = wt.Minutes.ToString("D2");
            //        //    var seconds = wt.Seconds;
            //        //    list.Add(
            //        //        $"Name {current.name} Прибыл: {preEvent.dateTime} Убыл: {current.dateTime} Итого: {hours.ToString("D0")}:{minutes}:{seconds.ToString("D2")}");
            //        //    //System.Diagnostics.Debug.WriteLine($"Name {current.name} Прибыл: {preEvent.dateTime} Убыл: {current.dateTime} Итого: {current.dateTime.Subtract(preEvent.dateTime)}");
            //        //}
            //        preEvent = current;
            //    }


                    
            //    Console.WriteLine();

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine();
            //}

            sw1.Stop();
            var time1 = sw1.ElapsedMilliseconds;

            System.Diagnostics.Debug.WriteLine($"---------------------{time1}-------------------");

            //Assert.Equal(result, res );
        }
    }
}
