using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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


        [HttpGet]
        [Route("report")]
        public async Task<Report> GetReportByReaders(DateTime startDate, DateTime endDate, [FromQuery]List<int> inputReader, [FromQuery]List<int> outputReader)
        {
            var result = await _reportService.GetReportByReaders(startDate, endDate, inputReader, outputReader);
            return result;
        }


        [HttpGet]
        [Route("send")]
        public async Task SendDocumentAsync()
        {
            var bot = new TelegramBotClient("6581396259:AAHak1OPEZiUJ5R0bSJDb3GZQe9MnSuuznc");
            await bot.SendTextMessageAsync("6310947780", "Как дела?");
        }

        [HttpGet]
        [Route("wd")]
        public async Task<List<DateTime>>GetWorkingDays(int uid, DateOnly date)
        {
            var result = await _reportService.GetWorkingDaysByUserId(uid, date.ToDateTime(TimeOnly.MinValue));
            return result;
        }

    }
}
