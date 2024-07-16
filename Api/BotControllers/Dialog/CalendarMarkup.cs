using System.Globalization;
using PRTelegramBot.Models.CallbackCommands;
using PRTelegramBot.Models.Enums;
using PRTelegramBot.Models.InlineButtons;
using PRTelegramBot.Utils;
using PRTelegramBot.Utils.Controls.CalendarControl.Common;
using Telegram.Bot.Types.ReplyMarkups;

namespace Api.BotControllers.Dialog;

public static class CalendarMarkup
{


    /// <summary>
    /// Разметка календаря.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <param name="dtfi">Формат даты.</param>
    /// <param name="command">Заголовок команды.</param>
    /// <returns>Inline меню.</returns>
    public static InlineKeyboardMarkup Calendar(in DateTime date, DateTimeFormatInfo dtfi, List<DateTime> workingDays, int userId, int command = 0)
    {
        var calendarRows = new List<IEnumerable<InlineKeyboardButton>>();
        calendarRows.Add(CalendarRow.Date(date, dtfi, userId, 0));
        calendarRows.Add(CalendarRow.DayOfWeek(dtfi, 0));
        var rows = CalendarRow.Month(date, dtfi, workingDays, userId, command);

        calendarRows.AddRange(rows);
        // добавить кнопки смены месяца <, Обновить >
        calendarRows.Add(CalendarRow.Controls(date, userId, command));

        return new InlineKeyboardMarkup(calendarRows);
    }


    public static InlineKeyboardMarkup PickMonthYear(DateTime date, DateTimeFormatInfo dtfi, int userId, int command)
    {
        var keyboardRows = new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>(date.ToString("MMMM", dtfi), PRTelegramBotCommand.PickMonth, new CustomCalendarCommand(date, userId, command))),
                MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>(date.ToString("yyyy", dtfi), PRTelegramBotCommand.PickYear, new CustomCalendarCommand(date, userId, command)))
            },
            new InlineKeyboardButton[]
            {
                MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>("<<", PRTelegramBotCommand.ChangeTo, new CustomCalendarCommand(date, userId, command))),
                " "
            }
        };

        return new InlineKeyboardMarkup(keyboardRows);
    }

    public static InlineKeyboardMarkup PickYear(in DateTime date, DateTimeFormatInfo dtfi, int userId, int command = 0)
    {
        var keyboardRows = new InlineKeyboardButton[6][];

        var startYear = date.AddYears(-7);

        for (int i = 0, row = 0; i < 12; row++)
        {
            var keyboardRow = new InlineKeyboardButton[3];
            for (var j = 0; j < 3; j++, i++)
            {
                var day = startYear.AddYears(i);
                keyboardRow[j] = MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>(day.ToString("yyyy", dtfi), PRTelegramBotCommand.YearMonthPicker, new CustomCalendarCommand(day, userId, command)));
            }

            keyboardRows[row] = keyboardRow;
        }
        keyboardRows[4] = CalendarRow.BackToMonthYearPicker(date, userId, command);
        keyboardRows[5] = CalendarRow.ChangeYear(date, userId, command);

        return new InlineKeyboardMarkup(keyboardRows);
    }

    /// <summary>
    /// Разметка выбора месяца.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <param name="dtfi">Формат даты.</param>
    /// <param name="command">Заголовок команды.</param>
    /// <returns>Inline меню.</returns>
    public static InlineKeyboardMarkup PickMonth(in DateTime date, DateTimeFormatInfo dtfi, int userId, int command = 0)
    {
        var keyboardRows = new InlineKeyboardButton[5][];

        for (int month = 0, row = 0; month < 12; row++)
        {
            var keyboardRow = new InlineKeyboardButton[3];
            for (var j = 0; j < 3; j++, month++)
            {
                var day = new DateTime(date.Year, month + 1, 1);

                keyboardRow[j] = MenuGenerator.GetInlineButton(new InlineCallback<CustomCalendarCommand>(dtfi.MonthNames[month], PRTelegramBotCommand.YearMonthPicker, new CustomCalendarCommand(day, userId, command)));
            }

            keyboardRows[row] = keyboardRow;
        }
        keyboardRows[4] = CalendarRow.BackToMonthYearPicker(date, userId, command);

        return new InlineKeyboardMarkup(keyboardRows);
    }
}