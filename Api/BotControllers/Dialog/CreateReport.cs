using Api.BotControllers.Keyboard;
using PRTelegramBot.Attributes;
using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;
using PRTelegramBot.Extensions;


namespace Api.BotControllers.Dialog
{
    [BotHandler]
    public class CreateReport
    {
        private const string SelectPeriodMsg = "Выберите период";

        [ReplyMenuHandler("Отчет", "КРАТКИЙ ОТЧЕТ")]
        public async Task QuickReportCommand(ITelegramBotClient client, Update update)
        {
            // клавиатура для быстрого отчета
            var option = Menu.ReportKeyboard(ReportType.Quick);
            update.GetCacheData<CreateReportCache>().Type = ReportType.Quick;

            await PRTelegramBot.Helpers.Message.Send(client, update, SelectPeriodMsg, option);
            
        }

        
        [ReplyMenuHandler("ПОДРОБНЫЙ ОТЧЕТ")]
        public async Task DetailedReportCommand(ITelegramBotClient client, Update update)
        {
            
            var optionMessage = Menu.ReportKeyboard(ReportType.Detailed);
            update.GetCacheData<CreateReportCache>().Type = ReportType.Detailed;

            await PRTelegramBot.Helpers.Message.Send(client, update, SelectPeriodMsg, optionMessage);
        }

    }
}
