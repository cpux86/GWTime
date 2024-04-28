using System.Globalization;
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
        public static IEnumerable<IEnumerable<InlineKeyboardButton>> Month(DateTime date, DateTimeFormatInfo dtfi, List<DateTime> dateTimes, int command = 0)
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
                    //var t1 = FirstDayOfWeek();
                    if (weekNum == 0 && dayOfWeek < FirstDayOfWeek()
                        ||
                        dayOfMonth > lastDayOfMonth
                       )
                    {
                        week[dayOfWeek] = " ";
                        continue;
                    }

                    //week[dayOfWeek] = dayOfMonth.ToString();
                    int day = dayOfMonth;
                    if (dateTimes.Exists(e => e.Day == day))
                    {
                        if (day == DateTime.Today.Day)
                        {
                            week[dayOfWeek] = MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>($"{day}", THeader.PickDate, new CalendarTCommand(new DateTime(date.Year, date.Month, dayOfMonth), command)));
                        }
                        else
                        {
                            week[dayOfWeek] = MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>($"{day}", THeader.PickDate, new CalendarTCommand(new DateTime(date.Year, date.Month, dayOfMonth), command)));
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




    }
}
