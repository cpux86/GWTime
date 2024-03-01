using Application.Interfaces;
using PRTelegramBot.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Api.BotControllers
{
    public class ReportHandler
    {
        private readonly IReportService _report;

        public ReportHandler(IReportService report)
        {
            _report = report;
        }

        //[ReplyMenuHandler("Отчет за сутки")]
        //public async Task ReportForDayAsync(ITelegramBotClient client, Update update)
        //{
        //    var res = await _report.GetReportByReaders(DateTime.Parse("2024-01-01 06:00:00.00"),
        //        DateTime.Now, new List<int> { 141 }, new List<int>() { 142 });
        //    var t = res.Groups.Select(e => e.Name);
        //    foreach (var g in t)
        //    {
        //        Console.WriteLine(g);
        //    }
           
        //}
    }
}
