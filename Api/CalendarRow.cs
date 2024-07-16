using System.Globalization;
using Api.BotControllers.Dialog;
using PRTelegramBot.Models.CallbackCommands;
using PRTelegramBot.Models.Enums;
using PRTelegramBot.Models.InlineButtons;
using PRTelegramBot.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Api
{
    public class CalendarRow 
    {


        /// <summary>
        /// Коллекция дней недели.
        /// </summary>
        /// <param name="dtfi">Формат даты.</param>
        /// <returns>Коллекция inline кнопок.</returns>
        public static IEnumerable<InlineKeyboardButton> DayOfWeek(DateTimeFormatInfo dtfi, int command = 0)
        {
            var dayNames = new InlineKeyboardButton[7];

            var firstDayOfWeek = (int)dtfi.FirstDayOfWeek;
            for (int i = 0; i < 7; i++)
            {
                yield return dtfi.AbbreviatedDayNames[(firstDayOfWeek + i) % 7];
            }
        }





        /// <summary>
        /// Коллекция месяцев
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="dtfi">Формат даты</param>
        /// <param name="workingDays">Дни которые нужно отобразить</param>
        /// <param name="userId"></param>
        /// <returns>Коллекция inline кнопок</returns>
        public static IEnumerable<IEnumerable<InlineKeyboardButton>> Month(DateTime date, DateTimeFormatInfo dtfi, List<DateTime> workingDays, int userId, int command = 0)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).Day;

            for (int dayOfMonth = 1, weekNum = 0; dayOfMonth <= lastDayOfMonth; weekNum++)
            {
                yield return NewWeek(weekNum, ref dayOfMonth);
            }

            IEnumerable<InlineKeyboardButton> NewWeek(int weekNum, ref int dayOfMonth)
            {
                var week = new InlineKeyboardButton[7];

                for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
                {
                    if (weekNum == 0 && dayOfWeek < FirstDayOfWeek()
                        ||
                        dayOfMonth > lastDayOfMonth
                       )
                    {
                        week[dayOfWeek] = " ";
                        continue;
                    }

                    var day = dayOfMonth;
                    if (workingDays.Exists(e => e.Day == day))
                    {
                        week[dayOfWeek] = MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>($"{day}", PRTelegramBotCommand.PickDate, new CustomCalendarCommand(new DateTime(date.Year, date.Month, dayOfMonth), userId, command)));
                    }
                       
                    else
                    {
                        week[dayOfWeek] = "-";
                    }
                    
                    
                    dayOfMonth++;
                }
                return week;

                int FirstDayOfWeek() =>
                    (7 + (int)firstDayOfMonth.DayOfWeek - (int)dtfi.FirstDayOfWeek) % 7;
            }
        }


         /// <summary>
        /// Генерация контролов для переходов по месяцам
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Коллекция inline кнопок</returns>
        public static IEnumerable<InlineKeyboardButton> Controls(in DateTime date,int userId, int command = 0) =>
            new InlineKeyboardButton[]
            {
                MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>("<", PRTelegramBotCommand.ChangeTo, new CustomCalendarCommand(date.AddMonths(-1),userId, command))),
                MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>("↻", PRTelegramBotCommand.ChangeTo, new CustomCalendarCommand(date,userId, command))),
                MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>(">", PRTelegramBotCommand.ChangeTo, new CustomCalendarCommand (date.AddMonths(1), userId, command))),
            };

         public static IEnumerable<InlineKeyboardButton> Date(DateTime date, DateTimeFormatInfo dtfi, int userId, int command = 0) => 
         
            new InlineKeyboardButton[]
            {
                MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>($"» {date.ToString("Y", dtfi)} «", PRTelegramBotCommand.YearMonthPicker, new CustomCalendarCommand(date, userId, command)))
            };




         /// <summary>
         /// Возращение к выбору месяца года
         /// </summary>
         /// <param name="date"></param>
         /// <returns>Массив inline кнопок</returns>
         public static InlineKeyboardButton[] BackToMonthYearPicker(in DateTime date, int userId, int command = 0) =>
             new InlineKeyboardButton[3]
             {
                 MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>("<<", PRTelegramBotCommand.YearMonthPicker, new CustomCalendarCommand(date, userId, command))),
                 " ",
                 " "
             };


         /// <summary>
         /// Смена года
         /// </summary>
         /// <param name="date">Дата</param>
         /// <returns>Массив inline кнопок</returns>
         public static InlineKeyboardButton[] ChangeYear(in DateTime date, int userId, int command = 0) =>
             new InlineKeyboardButton[3]
             {
                 MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>("<", PRTelegramBotCommand.PickYear, new CustomCalendarCommand(date.AddYears(-12), userId, command))),
                 " ",
                 MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>(">", PRTelegramBotCommand.PickYear, new CustomCalendarCommand(date.AddYears(12), userId, command)))
             };

    }
}
