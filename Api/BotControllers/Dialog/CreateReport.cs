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



        //Создаем список для меню
        List<KeyboardButton> menuList = new List<KeyboardButton>();

        public async Task SelectPeriod(ITelegramBotClient client, Update update)
        {
            
            string msg = "Выберите устройство регистрации!";
            //Создаем настройки сообщения
            var option = new OptionMessage();

            menuList.Add(new KeyboardButton("Хронос"));
            menuList.Add(new KeyboardButton("Центральный вход"));
            menuList.Add(new KeyboardButton("Курилка № 1"));
            menuList.Add(new KeyboardButton("Курилка № 2"));
            menuList.Add(new KeyboardButton("Гальваника"));


            var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");

            option.MenuReplyKeyboardMarkup = menu;

            await PRTelegramBot.Helpers.Message.Send(client, update, msg, option);

            var text = update.Message?.Text switch
            {
                //"Сегодня" => "today",
                //"Вчера" => "yesterday",
                "Вчера" => DateTime.Now
            };
            
            var handler = update.GetStepHandler<StepTelegram>();
            handler.GetCache<CreateReportCache>().Text = update.Message.Text;
            //handler.RegisterNextStep(SelectReader, DateTime.Now.AddMinutes(5));


            await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);





            //var handler = update.GetStepHandler<StepTelegram>();

        }

        public async Task SelectReader(ITelegramBotClient client, Update update)
        {
            var handler = update.GetStepHandler<StepTelegram>();
            var cache = handler.GetCache<CreateReportCache>();
            cache.Type = ReportType.Quick;
            // ✅
            menuList.Find(e => e.Text == update.Message.Text).Text = $"✅ {update.Message.Text}";

            var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");
            var option = new OptionMessage();
            option.MenuReplyKeyboardMarkup = menu;


            await PRTelegramBot.Helpers.Message.Send(client, update, cache.Text, option);
            //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.Code);
            //var handler = update.GetStepHandler<StepTelegram>();

        }
    }
}
