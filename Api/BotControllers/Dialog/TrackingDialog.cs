using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using Telegram.Bot;
using Helpers = PRTelegramBot.Helpers;
using Telegram.Bot.Types;
using PRTelegramBot.Utils.Controls.CalendarControl.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Globalization;
using System.Runtime.CompilerServices;
using PRTelegramBot.Models.CallbackCommands;
using PRTelegramBot.Models.Enums;
using PRTelegramBot.Models.InlineButtons;
using Api.Models;
using Application.Interfaces;
using Google.Protobuf.WellKnownTypes;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using PRTelegramBot.Utils;
using System.Collections.Generic;
using Domain;
using PRTelegramBot.Interfaces;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.Enums;
using GWT;
using NLog.Extensions.Logging;
using User = Domain.User;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.BotControllers.Dialog
{
   
    public class CustomCalendarCommand : CalendarTCommand
    {
        [JsonProperty("uid")]
        public int UserId { get; set; }
        public CustomCalendarCommand(DateTime date, int userId, int command = 0) : base(date, command)
        {
            UserId = userId;
        }
    }

    [BotHandler]
    public class TrackingDialog
    {
        private readonly IReportService _reportService;
        private readonly IUserManager _userManager;
        private readonly ILogger _logger;

        public TrackingDialog(IReportService reportService, IUserManager userManager, ILogger<TrackingDialog> logger)
        {
            this._reportService = reportService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Русский формат даты
        /// </summary>
        public static DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("ru-RU", false).DateTimeFormat;

        [ReplyMenuHandler("Трекинг", "/tracking")]
        public async Task Tracking(ITelegramBotClient client, Update update)
        {
            var msg = "Кто вас интересует?";

            update.RegisterStepHandler(new StepTelegram(FindUserByName, new TrackingCache()));
           
            var options = new OptionMessage();
            
            //var users = await _userManager.GetUserListAsync();

            var now = DateTime.Now;
            // Отчетная дата - начало каждого месяца. Это первое число плюс время
            var startDateTime = new DateTime(now.Year, now.Month, 1).AddMonths(-1);

            var users = await _reportService.GetUsersAsync(startDateTime, now);

            var userList = users.Take(250).DistinctBy(u=>u.Name).Select(user => new KeyboardButton($"{user.Name}")).ToList(); 

            var menu = MenuGenerator.ReplyKeyboard(2, userList, true, "Главное меню");

            options.MenuReplyKeyboardMarkup = menu;

            await Helpers.Message.Send(client, update, msg, options);
            
        }

        public async Task FindUserByName(ITelegramBotClient client, Update update)
        {
            // поиск по фамилии сотрудника в базе данных" 
            var userName = update.Message!.Text;
            _logger.LogInformation($"{update.GetInfoUser()}| Tracking по фамилии: {userName}");

            var users = await _userManager.GetUserByNameAsync(userName);


            if (users.Count > 1 )
            {
                var userList = users.Take(250).Select(user => new KeyboardButton($"{user.FullName}")).ToList();

                var menu = MenuGenerator.ReplyKeyboard(1, userList, true, "Главное меню");
                var options = new OptionMessage();

                options.MenuReplyKeyboardMarkup = menu;

                await Helpers.Message.Send(client, update, "Выберите сотрудника из списка или уточните запрос", options);
                return;
            }
            
            var user = users.LastOrDefault();

            if (user == null)
            {
                _logger.LogWarning($"User {userName} не найден");
                await PRTelegramBot.Helpers.Message.Send(client, update.GetChatId(), "По вашему запросу не кто не найден!");
                return;
            }

            // передаем id пользователя и нужный месяц
            var workingDays = _reportService.GetWorkingDaysByUserId(user.Id, DateTime.Today);

            //Получаем текущий обработчик
            var handler = update.GetStepHandler<StepTelegram>();
            handler!.GetCache<TrackingCache>().UserId = user.Id;
            //handler!.GetCache<TrackingCache>().FullName = user.FullName;

            // если сотрудника найден и он единственный по фамилии, то предлагаем выбрать дату 

            //var calendarMarkup = Markup.Calendar(DateTime.Today, dtfi);


            var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();
            
            keyboardRows.Add(Row.Date(DateTime.Today, dtfi, 0));
            keyboardRows.Add(Row.DayOfWeek(dtfi, 0));
            var testRows = TestRow.Month(DateTime.Today, dtfi, workingDays, user.Id, 0);
            
            keyboardRows.AddRange(testRows);
  
            keyboardRows.Add(TestRow.Controls(DateTime.Today,user.Id, 0));



            var option = new OptionMessage();
            option.MenuInlineKeyboardMarkup = new InlineKeyboardMarkup(keyboardRows);
            handler!.GetCache<TrackingCache>().Options = option;
            //option.MenuInlineKeyboardMarkup = calendarMarkup;

            //var msg = $"👤 <b>{user.FullName.Trim()}</b>\n" +
            //          $"👥 {user.Group!.Name}.\n" +
            //          $"последний доступ: {lastUseKey.Reader.Name}\n" +
            //          $"{lastUseKey.DateTime:f}";


            var msg = $"👤 <b>{user.FullName.Trim()}</b>\n" +
                      $"👥 {user.Group!.Name}.\n" +
                      $"последний доступ: {user.LastUsedReaderName}\n" +
                      $"{user.LastUsedKeyDate:f}";

            await Helpers.Message.Send(client, update.GetChatId(), msg, option);
        }

        /// <summary>
        /// Выбор года или месяца
        /// </summary>
        [InlineCallbackHandler<PRTelegramBotCommand>(PRTelegramBotCommand.YearMonthPicker)]
        public static async Task PickYearMonth(ITelegramBotClient botClient, Update update)
        {
            try
            {
                var command = InlineCallback<CalendarTCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                if (command != null)
                {
                    var monthYearMarkup = Markup.PickMonthYear(command.Data.Date, dtfi, command.Data.LastCommand);
                    var option = new OptionMessage();
                    option.MenuInlineKeyboardMarkup = monthYearMarkup;
                    await Helpers.Message.EditInline(botClient, update.GetChatId(), update.GetMessageId(), option);
                }
            }
            catch (Exception ex)
            {
                //Обработка исключения
            }
        }

        /// <summary>
        /// Выбор месяца
        /// </summary>
        [InlineCallbackHandler<PRTelegramBotCommand>(PRTelegramBotCommand.PickMonth)]
        public static async Task PickMonth(ITelegramBotClient botClient, Update update)
        {
            try
            {
                var command = InlineCallback<CalendarTCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                if (command != null)
                {
                    var monthPickerMarkup = Markup.PickMonth(command.Data.Date, dtfi, command.Data.LastCommand);
                    var option = new OptionMessage
                    {
                        MenuInlineKeyboardMarkup = monthPickerMarkup
                    };
                    await Helpers.Message.EditInline(botClient, update.GetChatId(), update.GetMessageId(), option);
                }


            }
            catch (Exception ex)
            {
                //Обработка исключения
            }
        }

        /// <summary>
        /// Выбор года
        /// </summary>
        [InlineCallbackHandler<PRTelegramBotCommand>(PRTelegramBotCommand.PickYear)]
        public static async Task PickYear(ITelegramBotClient botClient, Update update)
        {
            try
            {
                var command = InlineCallback<CalendarTCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                if (command != null)
                {
                    var monthYearMarkup = Markup.PickYear(command.Data.Date, dtfi, command.Data.LastCommand);
                    var option = new OptionMessage();
                    option.MenuInlineKeyboardMarkup = monthYearMarkup;
                    await Helpers.Message.EditInline(botClient, update.GetChatId(), update.GetMessageId(), option);
                }
            }
            catch (Exception ex)
            {
                //Обработка исключения
            }
        }

        // <summary>
        /// Перелистывание месяца
        /// </summary>
        [InlineCallbackHandler<PRTelegramBotCommand>(PRTelegramBotCommand.ChangeTo)]
        public  async Task ChangeToHandler(ITelegramBotClient botClient, Update update)
        {
            try
            {
                var command = InlineCallback<CustomCalendarCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                if (command != null)
                {
                    //var calendarMarkup = Markup.Calendar(command.Data.Date, dtfi, command.Data.LastCommand);
                    var option = new OptionMessage();



                    var handler = update.GetStepHandler<StepTelegram>();
                    if (handler == null)
                    {
                        await Tracking(botClient, update);
                        return;
                    }
                    var cache = handler!.GetCache<TrackingCache>();

                    //if (cache.UserId == 0)
                    //{
                    //    await Tracking(botClient, update);
                    //    return;
                    //}
                    if (command.Data.UserId == 0)
                    {
                        await Tracking(botClient, update);
                        return;
                    }

                    var user = await _userManager.GetUserByIdAsync(command.Data.UserId);

                    var workingDays = _reportService.GetWorkingDaysByUserId(user.Id, command.Data.Date);

                    //var workDateTimes = workingDays.Where(e => e.Date.ToString("Y") == command.Data.Date.ToString("Y")).ToList();


                    var calendarRows = new List<IEnumerable<InlineKeyboardButton>>();
                    calendarRows.Add(Row.Date(command.Data.Date, dtfi, 0));
                    calendarRows.Add(Row.DayOfWeek(dtfi, 0));
                    //var rows = TestRow.Month(command.Data.Date, dtfi, workDateTimes,command.Data.UserId, 0);
                    var rows = TestRow.Month(command.Data.Date, dtfi, workingDays, command.Data.UserId, 0);

                    calendarRows.AddRange(rows);
                    // добавить кнопки смены месяца <, Обновить >
                    calendarRows.Add(TestRow.Controls(command.Data.Date, command.Data.UserId, 0));

                    option.MenuInlineKeyboardMarkup = new InlineKeyboardMarkup(calendarRows);
                    handler!.GetCache<TrackingCache>().Options = option;

                    await Helpers.Message.EditInline(botClient, update.GetChatId(), update.GetMessageId(), option);
                }
            }
            catch (Exception ex)
            {
                //Обработка исключения
            }

        }

        /// <summary>
        /// Обработка выбранной даты 🔟
        /// </summary>
        [InlineCallbackHandler<PRTelegramBotCommand>(PRTelegramBotCommand.PickDate)]
        public async Task PickDate(ITelegramBotClient botClient, Update update)
        {
            var sb = new StringBuilder();
            try
            {
                //var command = InlineCallback<CalendarTCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                var command = InlineCallback<CustomCalendarCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);

                if (command != null)
                {
                    var calendarMarkup = Markup.Calendar(command.Data.Date, dtfi, command.Data.LastCommand);
                    

                    var option = new OptionMessage();
                    

                    //var res = Row.Month(DateTime.Now, dtfi);
                    option.MenuInlineKeyboardMarkup = calendarMarkup;

                    var data = command.Data.Date;
                    //var eventsTest = await _reportService.TrackingByUserIdAndDateAsync(command.Data.UserId, data);
                    var handler = update.GetStepHandler<StepTelegram>();
                    //update.GetCacheData<TrackingCache>();

                    

                    if (handler == null)
                    {
                        await Tracking(botClient, update);
                        return;
                    }

                    var cache = handler!.GetCache<TrackingCache>();

                    #region Потом можно удалить
                    var workingDays = _reportService.GetWorkingDaysByUserId(command.Data.UserId, command.Data.Date);

                    //var workDateTimes = workingDays.Where(e => e.Date.ToString("Y") == command.Data.Date.ToString("Y")).ToList();


                    var calendarRows = new List<IEnumerable<InlineKeyboardButton>>();
                    calendarRows.Add(Row.Date(command.Data.Date, dtfi, 0));
                    calendarRows.Add(Row.DayOfWeek(dtfi, 0));
                    //var rows = TestRow.Month(command.Data.Date, dtfi, workDateTimes,command.Data.UserId, 0);
                    var rows = TestRow.Month(command.Data.Date, dtfi, workingDays, command.Data.UserId, 0);

                    calendarRows.AddRange(rows);
                    // добавить кнопки смены месяца <, Обновить >
                    calendarRows.Add(TestRow.Controls(command.Data.Date, command.Data.UserId, 0));

                    option.MenuInlineKeyboardMarkup = new InlineKeyboardMarkup(calendarRows);
                    handler!.GetCache<TrackingCache>().Options = option;


                    #endregion
                    

                    //if (cache.UserId == 0)
                    //{
                    //    await Tracking(botClient, update);
                    //    return;
                    //}

                    //Обработка данных даты;
                    //var events = await _reportService.TrackingByUserIdAndDateAsync(cache.UserId, data);
                    var events = await _reportService.TrackingByUserIdAndDateAsync(command.Data.UserId, data);
                    var fullName = events[0].User.FullName;

                    sb.Append($"<u><b>Отчет за {command.Data.Date.ToString("d")}</b></u>\n");
                    //sb.Append($"<b>{cache.FullName}</b>\n");
                    sb.Append($"<b>{fullName}</b>\n");

                    //var t1 = sb.AppendJoin("\n", events.OrderBy(e => e.DateTime)
                    //    .Select(e => $"[{e.DateTime:t}] {e.Reader.Name}"));

                    //var t = sb.ToString();
                    //Console.WriteLine(t.Length);
#if DEBUG
                    //botClient.InvokeCommonLog(t);
#endif
                    //await Helpers.Message.Edit(botClient, update, sb.ToString(), cache.Options);
                    await Helpers.Message.Edit(botClient, update, sb.ToString(), option);

                    //await Helpers.Message.Edit(botClient, update.GetChatId(), update.GetMessageId(), sb.ToString(), cache.Options);
                    //await Helpers.Message.Edit(botClient, update.GetChatId(), update.GetMessageId(), sb.ToString());
                    botClient.InvokeCommonLog($"{update.GetInfoUser()}| Tracking по фамилии: {cache.FullName}, {command.Data.Date:d}");

                }
            }
            catch (Exception ex)
            {
                //Обработка исключения
            }
            finally
            {

            }
        }



    }
}
