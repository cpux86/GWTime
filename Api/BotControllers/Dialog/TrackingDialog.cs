using Application.Interfaces;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using PRTelegramBot.Models.Enums;
using PRTelegramBot.Models.InlineButtons;
using PRTelegramBot.Utils;
using System.Globalization;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Helpers = PRTelegramBot.Helpers;

namespace Api.BotControllers.Dialog
{
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

            //update.RegisterStepHandler(new StepTelegram(FindUserByName, new TrackingCache()));
            update.RegisterStepHandler(new StepTelegram(FindUserByName));

            var options = new OptionMessage();

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

            // если по запросу найдено больше одного человека, по предлагаю уточнить запрос или выбрать фио из предложенных. 
            if (users.Count > 1 )
            {
                var userList = users.Take(250).Select(user => new KeyboardButton($"u{user.Id}_{user.FullName}")).ToList();

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
                await PRTelegramBot.Helpers.Message.Send(client, update.GetChatId(), "Запрос не дал результата!");
                return;
            }
            // передаем id пользователя и нужный месяц
            var workingDays = await _reportService.GetWorkingDaysByUserId(user.Id, DateTime.Today);


            var calendarMarkup = CalendarMarkup.Calendar(DateTime.Today, dtfi, workingDays, user.Id);
            var option = new OptionMessage();
            option.MenuInlineKeyboardMarkup = calendarMarkup;

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
                var command = InlineCallback<CustomCalendarCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                if (command != null)
                {
                    var monthYearMarkup = CalendarMarkup.PickMonthYear(command.Data.Date, dtfi,command.Data.UserId, command.Data.LastCommand);
                    var option = new OptionMessage
                    {
                        MenuInlineKeyboardMarkup = monthYearMarkup
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
        /// Выбор месяца
        /// </summary>
        [InlineCallbackHandler<PRTelegramBotCommand>(PRTelegramBotCommand.PickMonth)]
        public static async Task PickMonth(ITelegramBotClient botClient, Update update)
        {
            try
            {
                var command = InlineCallback<CustomCalendarCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                if (command != null)
                {
                    var monthPickerMarkup = CalendarMarkup.PickMonth(command.Data.Date, dtfi, command.Data.UserId, command.Data.LastCommand);
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
                var command = InlineCallback<CustomCalendarCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);
                if (command != null)
                {
                    var monthYearMarkup = CalendarMarkup.PickYear(command.Data.Date, dtfi, command.Data.UserId, command.Data.LastCommand);
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
                //if (command != null)
                {
                    if (command.Data.UserId == 0)
                    {
                        await Tracking(botClient, update);
                        return;
                    }

                    var workingDays = await _reportService.GetWorkingDaysByUserId(command.Data.UserId, command.Data.Date);

                    var option = new OptionMessage();

                    var calendarMarkup = CalendarMarkup.Calendar(command.Data.Date, dtfi, workingDays, command.Data.UserId, command.Data.LastCommand);
                    option.MenuInlineKeyboardMarkup = calendarMarkup;

                    await Helpers.Message.EditInline(botClient, update.GetChatId(), update.GetMessageId(), option);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                var command = InlineCallback<CustomCalendarCommand>.GetCommandByCallbackOrNull(update.CallbackQuery.Data);

                if (command != null)
                {

                    var data = command.Data.Date;

                    var workingDays = await _reportService.GetWorkingDaysByUserId(command.Data.UserId, command.Data.Date);
                    var calendarMarkup = CalendarMarkup.Calendar(command.Data.Date, dtfi, workingDays, command.Data.UserId, command.Data.LastCommand);
                    var option = new OptionMessage
                    {
                        MenuInlineKeyboardMarkup = calendarMarkup
                    };

                    var events = await _reportService.TrackingByUserIdAndDateAsync(command.Data.UserId, command.Data.Date);
                    var fullName = events[0].User!.FullName;

                    sb.Append($"<u><b>Отчет за {command.Data.Date:D}</b></u>\n");
                    sb.Append($"<b>{fullName}</b>\n");
                    sb.AppendJoin("\n", events.OrderBy(e => e.DateTime)
                        .Select(e => $"[{e.DateTime:t}] {e.Reader.Name}")
                        .Distinct()
                    );
                    await Helpers.Message.Edit(botClient, update, sb.ToString(), option);
                    botClient.InvokeCommonLog($"{update.GetInfoUser()}| Tracking по фамилии: {fullName}, {command.Data.Date:d}");

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
