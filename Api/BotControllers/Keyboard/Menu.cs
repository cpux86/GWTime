using Microsoft.EntityFrameworkCore.Diagnostics;
using PRTelegramBot.Models;
using PRTelegramBot.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Api.BotControllers.Keyboard
{
    public struct Menu
    {
        /// <summary>
        /// Главное меню
        /// </summary>
        /// <returns></returns>
        public static OptionMessage MainMenuKeyboard()
        {
            

            var menuList = new List<KeyboardButton>
            {
                new KeyboardButton("👷‍♂️Кто на работе"),
                new KeyboardButton("Отчет"),
                new KeyboardButton("🟢 Online"),
                new KeyboardButton("🛠 Настройка"),
                new KeyboardButton("Трекинг"),
                new KeyboardButton("commands")
            };

            var menu = MenuGenerator.ReplyKeyboard(2, menuList, true);

            var option = new OptionMessage
            {
                MenuReplyKeyboardMarkup = menu
            };
            return option;
        }


        //private static List<KeyboardButton> SettingsKeyboard => new()
        //{
        //    new KeyboardButton("Мои люди"),
        //    //new KeyboardButton("Другой"),

        //    //new KeyboardButton("По сотруднику"),
        //    //new KeyboardButton("По группе"),
        //    //new KeyboardButton("По всем"),
        //};


        public static OptionMessage SettingsKeyboard()
        {
            var option = new OptionMessage();

            var menu = new List<KeyboardButton>()
            {
                new KeyboardButton("Мои люди"),
            };

            option.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(2, menu, true, "Главное меню");
            return option;
        }
        // Клавиатура мои люди
        public static OptionMessage MyPeopleKeyboard()
        {
            var option = new OptionMessage();

            //var menu = new List<KeyboardButton>()
            //{
            //    new KeyboardButton("Показать список"),
            //    new KeyboardButton("Добавить"),
            //    new KeyboardButton("Удалить"),

            //};

            InlineKeyboardMarkup menu = new(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Редактировать", callbackData: "21"),
                },
                //new []
                //{
                //    InlineKeyboardButton.WithCallbackData(text: "↔", callbackData: "21"),
                //},
            });

            option.MenuInlineKeyboardMarkup = menu;

            //option.MenuReplyKeyboardMarkup = MenuGenerator.ReplyKeyboard(1, menu, true, "Назад");
            
            return option;
        }



        private static List<KeyboardButton> PeriodMenu => new()
        {
            new KeyboardButton("Сегодня"),
            new KeyboardButton("Вчера"),
            //new KeyboardButton("Текущая неделя"),
            //new KeyboardButton("Предыдущая неделя"),
            new KeyboardButton("1-я пол. месяца"),
            new KeyboardButton("2-я пол. месяца"),
            new KeyboardButton("Текущий месяц"),
            new KeyboardButton("Предыдущий месяц"),
        };

        ///// <summary>
        ///// Клавиатура краткого отчета
        ///// </summary>
        ///// <returns></returns>
        //public static OptionMessage QuickReportKeyboard()
        //{
        //    var periodMenu = PeriodMenu;
        //    var option = new OptionMessage();
        //    periodMenu.Add(new KeyboardButton("ПОДРОБНЫЙ ОТЧЕТ"));
        //    var menu = MenuGenerator.ReplyKeyboard(2, periodMenu, true, "Главное меню");

        //    option.MenuReplyKeyboardMarkup = menu;
        //    return option;
        //}

        ///// <summary>
        ///// Клавиатура подробного отчета
        ///// </summary>
        ///// <returns></returns>
        //public static OptionMessage DetailsReportKeyboard()
        //{
        //    var periodMenu = PeriodMenu;
        //    var option = new OptionMessage();
        //    periodMenu.Add(new KeyboardButton("КРАТКИЙ ОТЧЕТ"));
        //    var menu = MenuGenerator.ReplyKeyboard(2, periodMenu, true, "Главное меню");

        //    option.MenuReplyKeyboardMarkup = menu;
        //    return option;
        //}

        internal static OptionMessage ReportKeyboard(ReportType type)
        {
            var periodMenu = PeriodMenu;
            var option = new OptionMessage();

            switch (type)
            {
                case ReportType.Quick:
                    periodMenu.Add(new KeyboardButton("ПОДРОБНЫЙ ОТЧЕТ"));
                    break;
                case ReportType.Detailed:
                    periodMenu.Add(new KeyboardButton("КРАТКИЙ ОТЧЕТ"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            var menu = MenuGenerator.ReplyKeyboard(2, periodMenu, true, "Главное меню");
            option.MenuReplyKeyboardMarkup = menu;
            return option;
        }
    }
}
