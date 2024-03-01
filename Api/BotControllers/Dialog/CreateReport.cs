using PRTelegramBot.Attributes;
using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;
using PRTelegramBot.Extensions;
using System.Threading;
using Google.Protobuf.WellKnownTypes;

namespace Api.BotControllers.Dialog
{
    public class CreateReport
    {
        [ReplyMenuHandler("Отчет")]
        public async Task StartDialogAsync(ITelegramBotClient client, Update update)
        {
            var msg = "Выберите период";
            //Создаем настройки сообщения
            var option = new OptionMessage();

            //Создаем список для меню
            var menuList = new List<KeyboardButton>
            {
                new KeyboardButton("Сегодня"),
                new KeyboardButton("Вчера"),
                new KeyboardButton("Текущая неделя"),
                new KeyboardButton("Предыдущая неделя"),
                new KeyboardButton("Текущий месяц"),
                new KeyboardButton("Предыдущий месяц"),
                new KeyboardButton("Другое")
            };


            var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");

            option.MenuReplyKeyboardMarkup = menu;

            update.RegisterStepHandler(new StepTelegram(SelectPeriod, new ReportCache()));

            await PRTelegramBot.Helpers.Message.Send(client, update, msg, option);
            await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);

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
            handler.GetCache<ReportCache>().Text = update.Message.Text;
            handler.RegisterNextStep(SelectReader, DateTime.Now.AddMinutes(5));


            await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);





            //var handler = update.GetStepHandler<StepTelegram>();

        }

        public async Task SelectReader(ITelegramBotClient client, Update update)
        {
            var handler = update.GetStepHandler<StepTelegram>();
            var cache = handler.GetCache<ReportCache>();
            // ✅
            menuList.Find(e => e.Text == update.Message.Text).Text = $"✅ {update.Message.Text}";

            var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");
            var option = new OptionMessage();
            option.MenuReplyKeyboardMarkup = menu;
           

            await PRTelegramBot.Helpers.Message.Send(client, update, cache.Text, option);
            //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);
            //var handler = update.GetStepHandler<StepTelegram>();

        }
    }
}
