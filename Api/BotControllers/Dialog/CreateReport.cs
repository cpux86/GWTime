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
        [ReplyMenuHandler("Отчет")]
        public async Task StartDialogAsync(ITelegramBotClient client, Update update)
        {
            //var msg = "Выберите период";
            ////Создаем настройки сообщения
            //var option = new OptionMessage();

            ////Создаем список для меню
            //var menuList = new List<KeyboardButton>
            //{
            //    new KeyboardButton("Сегодня"),
            //    new KeyboardButton("Вчера"),
            //    new KeyboardButton("Текущая неделя"),
            //    new KeyboardButton("Предыдущая неделя"),
            //    new KeyboardButton("1-я пол. месяца"),
            //    new KeyboardButton("2-я пол. месяца"),
            //    new KeyboardButton("Текущий месяц"),
            //    new KeyboardButton("Предыдущий месяц"),
            //    //new KeyboardButton("ПОДРОБНЫЙ ОТЧЕТ")
            //    //new KeyboardButton("Другое")
            //};

            //menuList.Add(new KeyboardButton("ПОДРОБНЫЙ ОТЧЕТ"));

            //var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");

            //option.MenuReplyKeyboardMarkup = menu;

            ////update.RegisterStepHandler(new StepTelegram(SelectPeriod, new CreateReportCache()));

            //await PRTelegramBot.Helpers.Message.Send(client, update, msg, option);
            //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);

            await DetailsReportCommand(client, update);

        }




        //public List<KeyboardButton> PeriodMenu => new List<KeyboardButton>
        //{
        //    new KeyboardButton("Сегодня"),
        //    new KeyboardButton("Вчера"),
        //    new KeyboardButton("Текущая неделя"),
        //    new KeyboardButton("Предыдущая неделя"),
        //    new KeyboardButton("1-я пол. месяца"),
        //    new KeyboardButton("2-я пол. месяца"),
        //    new KeyboardButton("Текущий месяц"),
        //    new KeyboardButton("Предыдущий месяц"),
        //};


        [ReplyMenuHandler("ПОДРОБНЫЙ ОТЧЕТ")]
        public async Task QuickReportCommand(ITelegramBotClient client, Update update)
        {
            var msg = "Выберите период";
            //Создаем настройки сообщения
            //var option = new OptionMessage();

            //Создаем список для меню
            //var menuList = new List<KeyboardButton>
            //{
            //    new KeyboardButton("Сегодня"),
            //    new KeyboardButton("Вчера"),
            //    new KeyboardButton("Текущая неделя"),
            //    new KeyboardButton("Предыдущая неделя"),
            //    new KeyboardButton("1-я пол. месяца"),
            //    new KeyboardButton("2-я пол. месяца"),
            //    new KeyboardButton("Текущий месяц"),
            //    new KeyboardButton("Предыдущий месяц"),
            //};

            //menuList = PeriodMenu;


            //menuList.Add(new KeyboardButton("КРАТКИЙ ОТЧЕТ"));

            //var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");

            //option.MenuReplyKeyboardMarkup = menu;

            //update.RegisterStepHandler(new StepTelegram(SelectPeriod, new CreateReportCache()));

            var optionMessage = Menu.DetailsReportKeyboard();

            await PRTelegramBot.Helpers.Message.Send(client, update, msg, optionMessage);
            //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);
        }

        [ReplyMenuHandler("КРАТКИЙ ОТЧЕТ")]
        public async Task DetailsReportCommand(ITelegramBotClient client, Update update)
        {
            var msg = "Выберите период";
            //Создаем настройки сообщения
            //var option = new OptionMessage();

            //Создаем список для меню
            //var menuList = new List<KeyboardButton>
            //{
            //    new KeyboardButton("Сегодня"),
            //    new KeyboardButton("Вчера"),
            //    new KeyboardButton("Текущая неделя"),
            //    new KeyboardButton("Предыдущая неделя"),
            //    new KeyboardButton("1-я пол. месяца"),
            //    new KeyboardButton("2-я пол. месяца"),
            //    new KeyboardButton("Текущий месяц"),
            //    new KeyboardButton("Предыдущий месяц"),
            //};
            //menuList = PeriodMenu;
            //menuList.Add(new KeyboardButton("ПОДРОБНЫЙ ОТЧЕТ"));

            //var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");

            //option.MenuReplyKeyboardMarkup = menu;

            //update.RegisterStepHandler(new StepTelegram(SelectPeriod, new CreateReportCache()));

            var optionMessage = Menu.QuickReportKeyboard();

            await PRTelegramBot.Helpers.Message.Send(client, update, msg, optionMessage);
            //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);
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
