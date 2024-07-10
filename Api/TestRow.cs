using System.Globalization;
using Api.BotControllers.Dialog;
using PRTelegramBot.Models.CallbackCommands;
using PRTelegramBot.Models.Enums;
using PRTelegramBot.Models.InlineButtons;
using PRTelegramBot.Utils;
using Telegram.Bot.Types.ReplyMarkups;

namespace Api
{
    public class TestRow
    {

        /// <summary>
        /// Коллекция месецов
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="dtfi">Формат даты</param>
        /// <returns>Коллекция inline кнопок</returns>
        public static IEnumerable<IEnumerable<InlineKeyboardButton>> Month(DateTime date, DateTimeFormatInfo dtfi, List<DateTime> dateTimes, int userId, int command = 0)
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

                    int day = dayOfMonth;
                    if (dateTimes.Exists(e => e.Day == day))
                    {
                        if (day == DateTime.Today.Day)
                        {
                            //week[dayOfWeek] = MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>($"{day}", THeader.PickDate, new CalendarTCommand(new DateTime(date.Year, date.Month, dayOfMonth), command)));
                            week[dayOfWeek] = MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>($"{day}", PRTelegramBotCommand.PickDate, new CustomCalendarCommand(new DateTime(date.Year, date.Month, dayOfMonth), userId, command)));
                        }
                        else
                        {
                            //week[dayOfWeek] = MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>($"{day}", THeader.PickDate, new CalendarTCommand(new DateTime(date.Year, date.Month, dayOfMonth), command)));
                            week[dayOfWeek] = MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>($"{day}", PRTelegramBotCommand.PickDate, new CustomCalendarCommand(new DateTime(date.Year, date.Month, dayOfMonth), userId, command)));
                        }
                    }
                       
                    else
                    {
                        week[dayOfWeek] = "-";
                        // week[dayOfWeek] = MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>($"{day.ToString()}", THeader.PickDate, new CalendarTCommand(new DateTime(date.Year, date.Month, dayOfMonth), command)));
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
                MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>("<", PRTelegramBotCommand.ChangeTo, new CustomCalendarCommand(date.AddMonths(-1),userId, command))),
                MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>("ОБНОВИТЬ", PRTelegramBotCommand.ChangeTo, new CustomCalendarCommand(date,userId, command))),
                MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>(">", PRTelegramBotCommand.ChangeTo, new CustomCalendarCommand (date.AddMonths(1), userId, command))),
            };

    }
}
