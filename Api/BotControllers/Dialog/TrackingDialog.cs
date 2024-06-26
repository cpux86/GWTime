﻿using PRTelegramBot.Attributes;
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
using PRTelegramBot.Interface;
using System.Diagnostics;
using Telegram.Bot.Types.Enums;
using GWT;
using NLog.Extensions.Logging;
using User = Domain.User;

namespace Api.BotControllers.Dialog
{

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


            //_logger.LogError("Это ошибка");
            var msg = "Кто вас интересует?";

            
            update.RegisterStepHandler(new StepTelegram(FindUserByName, new TrackingCache()));
           
            var options = new OptionMessage();
            
            //var users = await _userManager.GetUserListAsync();

            var now = DateTime.Now;
            // Отчетная дата - начало каждого месяца. Это первое число плюс время
            var startDateTime = new DateTime(now.Year, now.Month, 1).AddMonths(-1);

            var users = await _reportService.GetUsersAsync(startDateTime, now);

            var userList = users.Take(250).Select(user => new KeyboardButton($"{user.Name}")).ToList(); ;


            var menu = MenuGenerator.ReplyKeyboard(2, userList, true, "Главное меню");


            options.MenuReplyKeyboardMarkup = menu;

            await Helpers.Message.Send(client, update, msg, options);
            
        }

        public async Task FindUserByName(ITelegramBotClient client, Update update)
        {
            
            // поиск по фамилии сотрудника в базе данных" 
            var userName = update.Message!.Text;
            //_logger.LogError($"Find user {userName}");
            _logger.LogInformation($"{update.GetInfoUser()}| Tracking по фамилии: {userName}");
            //Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            //stopwatch.Start();

            var users = await _userManager.GetUserByNameAsync(userName);

            //foreach (var usr in users)
            //{
            //    Console.WriteLine(usr.FullName);
            //}

            

            var dictionary = users.ToDictionary(s => s.Key, e => e.FullName);
            var user = users.LastOrDefault();

            if (user == null)
            {
                _logger.LogWarning($"User {userName} не найден");
                await PRTelegramBot.Helpers.Message.Send(client, update.GetChatId(), "не найдено");
                return;
            }
            //var workingDaysList = user.GetWorkingDaysList();
            // передаем id пользователя и нужный месяц
            var workingDays = _reportService.GetWorkingDaysByUserId(user.Id, DateTime.Today);

            //var workDateTimes = workingDaysList.Where(e => e.Month == DateTime.Today.Month).ToList();
            //var workDateTimes = workingDays.Where(e => e.Month == DateTime.Today.Month).ToList();

            //stopwatch.Start();
            //Получаем текущий обработчик
            var handler = update.GetStepHandler<StepTelegram>();
            handler!.GetCache<TrackingCache>().UserId = user.Id;
            handler!.GetCache<TrackingCache>().FullName = user.FullName;
            // если сотрудника найден и он единственный по фамилии, то предлагаем выбрать дату 

            //var calendarMarkup = Markup.Calendar(DateTime.Today, dtfi);


            var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();
            
            keyboardRows.Add(Row.Date(DateTime.Today, dtfi, 0));
            keyboardRows.Add(Row.DayOfWeek(dtfi, 0));
            //var testRows = Row.Month(DateTime.Today, dtfi, 0);
            var testRows = TestRow.Month(DateTime.Today, dtfi, workingDays, 0);
            
            keyboardRows.AddRange(testRows);
            keyboardRows.Add(Row.Controls(DateTime.Today, 0));


            //var lastUseKey = await _reportService.GetLastUseKey(user.Id);

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
        [InlineCallbackHandler<THeader>(THeader.YearMonthPicker)]
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
        [InlineCallbackHandler<THeader>(THeader.PickMonth)]
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
        [InlineCallbackHandler<THeader>(THeader.PickYear)]
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
                    await Helpers.Message.EditInline(botClient, update.GetChatId(), update.CallbackQuery.Message.MessageId, option);
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
        [InlineCallbackHandler<THeader>(THeader.ChangeTo)]
        public  async Task ChangeToHandler(ITelegramBotClient botClient, Update update)
        {
            try
            {
                var command = InlineCallback<CalendarTCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
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

                    if (cache.UserId == 0)
                    {
                        await Tracking(botClient, update);
                        return;
                    }

                    //var user = await _userManager.GetUserByNameAsync(cache.FullName);
                    var user = await _userManager.GetUserByIdAsync(cache.UserId);

                    //var workingDaysList = user.GetWorkingDaysList();
                    //var workDateTimes = workingDaysList.Where(e => e.Date.ToString("Y") == command.Data.Date.ToString("Y")).ToList();


                    var workingDays = _reportService.GetWorkingDaysByUserId(user.Id, command.Data.Date);

                    //var workDateTimes = workingDaysList.Where(e => e.Month == DateTime.Today.Month).ToList();
                    var workDateTimes = workingDays.Where(e => e.Date.ToString("Y") == command.Data.Date.ToString("Y")).ToList();


                    var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();
                    keyboardRows.Add(Row.Date(command.Data.Date, dtfi, 0));
                    keyboardRows.Add(Row.DayOfWeek(dtfi, 0));
                    //var testRows = Row.Month(DateTime.Today, dtfi, 0);
                    var testRows = TestRow.Month(command.Data.Date, dtfi, workDateTimes, 0);

                    keyboardRows.AddRange(testRows);
                    keyboardRows.Add(Row.Controls(command.Data.Date, 0));

                    // var t = MenuGenerator.GetInlineButton(new InlineCallback<CalendarTCommand>($"({1})", THeader.PickDate, new CalendarTCommand(new DateTime(command.Data.Date.Year, command.Data.Date.Month, 1), 0)));
                    option.MenuInlineKeyboardMarkup = new InlineKeyboardMarkup(keyboardRows);
                    handler!.GetCache<TrackingCache>().Options = option;
                    //option.MenuInlineKeyboardMarkup = t;
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
        [InlineCallbackHandler<THeader>(THeader.PickDate)]
        public async Task PickDate(ITelegramBotClient botClient, Update update)
        {
            var sb = new StringBuilder();
            try
            {
                var command = InlineCallback<CalendarTCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                if (command != null)
                {
                    var calendarMarkup = Markup.Calendar(command.Data.Date, dtfi, command.Data.LastCommand);
                    

                    var option = new OptionMessage();
                    

                    var res = Row.Month(DateTime.Now, dtfi);
                    option.MenuInlineKeyboardMarkup = calendarMarkup;

                    var type = command.Data.GetLastCommandEnum<CustomTHeader>();
                    var data = command.Data.Date;
                    var handler = update.GetStepHandler<StepTelegram>();
                    if (handler == null)
                    {
                        await Tracking(botClient, update);
                        return;
                    }

                    var cache = handler!.GetCache<TrackingCache>();
                    if (cache.UserId == 0)
                    {
                        await Tracking(botClient, update);
                        return;
                    }

                    //Обработка данных даты;
                    var events = await _reportService.TrackingByUserIdAndDateAsync(cache.UserId, data);
                    //sb = new StringBuilder();
                    sb.Append($"<u><b>Отчет за {command.Data.Date.ToString("d")}</b></u>\n");
                    sb.Append($"<b>{cache.FullName}</b>\n");

                    var t1 = sb.AppendJoin("\n", events.OrderBy(e => e.DateTime)
                        .Select(e => $"[{e.DateTime:t}] {e.Reader.Name}"));

                    //var t = sb.ToString();
                    //Console.WriteLine(t.Length);
#if DEBUG
                    //botClient.InvokeCommonLog(t);
#endif

                    await Helpers.Message.Edit(botClient, update.GetChatId(), update.GetMessageId(), sb.ToString(), cache.Options);
                    botClient.InvokeCommonLog($"{update.GetInfoUser()}| Tracking по фамилии: {cache.FullName}, {command.Data.Date:d}");
                    //_logger.LogInformation($"{update.GetInfoUser()}| Tracking по фамилии: {cache.FullName}, {command.Data.Date:d}");

                }
            }
            catch (Exception ex)
            {
                //Обработка исключения
            }
            finally
            {
                //long totalMemory = GC.GetTotalMemory(false);

                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //sb.Clear();
            }
        }



    }
}
