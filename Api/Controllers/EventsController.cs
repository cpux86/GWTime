using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Api.Models;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Api.Controllers
{
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public EventsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        //[HttpGet]
        //[Route("{userId:int}")]
        //public async Task<List<Event>> GetEventsByUserId(int inputReader, int outputReader, int userId)
        //{
        //    var sw = Stopwatch.StartNew();
        //    var res = await _eventsService.GetEventsByUserId(inputReader, outputReader, userId);
        //    sw.Stop();
        //    Console.WriteLine("Finished in " + sw.Elapsed);
        //    return res;
        //}
            

        [HttpGet]
        [Route("/")]
        public async Task GetEvetn()
        {
            var ps = Process.Start("EventLoggerClientApp.exe", " /host 10.65.68.210");
            var info = ps.StartInfo.Arguments;

            //Task.Delay(10000).Wait();
            //ps.Close();
        }

        [HttpPost]
        [Route("ps")]
        public async Task GetRoutPoints([FromBody] List<WorkingDays> workingDaysList)
        {
            Console.WriteLine();
        }







        //[HttpGet]
        //[Route("users")]
        //public async Task<List<User>> GetUserListAsync()
        //{
        //    return await _eventsService.GetUserListAsync();
        //}

        //[HttpGet]
        //[Route("users/{name}")]
        //public async Task<List<User>> GetUserByNameAsync(string name)
        //{
        //    return await _reportService.GetUserByNameAsync(name);
        //}

        //[HttpGet]
        //[Route("user/time")]
        //public async Task<List<Event>> GetFirstAndLastUseKey(int userId, DateTime startDate, DateTime endDate)
        //{
        //    return await _reportService.GetFirstAndLastUseKey(userId, startDate, endDate);
        //}

        [HttpGet]
        [Route("report")]
        public async Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, [FromQuery]List<int> inputReader, [FromQuery]List<int> outputReader)
        {
            var result = await _reportService.GetReportByReaders(startDate, endDate, inputReader, outputReader);
            return result;
        }

        //[HttpGet]
        //[Route("tracking")]
        //public async Task<List<User>> GetUserListWithEventsByDateRange(DateTime startDate, DateTime endDate)
        //{
        //    var t = await _reportService.GetUserListWithEventsByDateRange(startDate, endDate);
        //    return t;
        //}

        [HttpGet]
        [Route("send")]
        public async Task SendDocumentAsync()
        {
            var bot = new TelegramBotClient("6581396259:AAHak1OPEZiUJ5R0bSJDb3GZQe9MnSuuznc");
            await bot.SendTextMessageAsync("6310947780", "Как дела?");
        }
    }
}
