using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
        [HttpGet]
        [Route("ps")]
        public async Task<List<int>> GetProcess()
        {

            var pss = Process.GetProcessesByName("EventLoggerClientApp");

            var ts = pss.AsEnumerable().Select(x => x.Id).ToList();

            foreach (var ps in pss)
            {
                //var arguments = ps.StartInfo.Arguments;
                ps.Kill();
            }

            return ts;
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
        //    return await _eventsService.GetUserByNameAsync(name);
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
            return await _reportService.GetReportByReaders(startDate, endDate, inputReader, outputReader);
        }
    }
}
