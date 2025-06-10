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
    }
}
