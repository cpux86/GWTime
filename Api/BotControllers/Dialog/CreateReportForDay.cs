﻿using Api.Models;
using Application.Interfaces;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using PRTelegramBot.Models.InlineButtons;
using PRTelegramBot.Utils;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Diagnostics;
using System.Globalization;
using Spire.Xls;
using System.Data;
using Api.BotControllers.Keyboard;
using Microsoft.IdentityModel.Tokens;
using PRTelegramBot.Utils.Controls.CalendarControl.Common;
using Group = Application.DTOs.Group;
using GWT;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
using Spire.Xls.Core;
using User = Domain.User;
using Application.DTOs;


namespace Api.BotControllers.Dialog;
[BotHandler]
public class CreateReportForDay
{
    private readonly IReportService _report;
    private readonly IUserManager _userManager;
    private readonly ILogger _logger;
    private readonly GWT.Client _client;
    public CreateReportForDay(IReportService report, IUserManager userManager, ILogger<CreateReportForDay> logger)
    {
        _report = report;
        _userManager = userManager;
        _logger = logger;
        _client = new GWT.Client("http://localhost:55222", new HttpClient());
    }

    //[ReplyMenuHandler("/start")]
    //[ReplyMenuHandler("Хронос")]
    //public async Task Start(ITelegramBotClient client, Update update)
    //{
    //    //var report = await _report.GetReportByReaders(DateTime.Parse("2024-01-01 06:00:00.00"),
    //    //    DateTime.Now, new List<int> { 141 }, new List<int>() { 142 });
    //    //var userList = report.Workers.Where(e => e.WorkTimes.Count > 0).ToList();
    //    //var groups= userList.GroupBy(e => e.Group.Name).Select(e => new Group() { Name = e.Key, Workers = e.ToList() }).ToList();

    //    //foreach (var group in groups)
    //    //{
    //    //    var msg = new StringBuilder();
    //    //    msg.Append($"<b>#⃣ {group.Name} #⃣</b>\n\r");
    //    //    msg.AppendJoin("\n", group.Workers.Select(e => e.Name = $"🙎‍♂️ {e.Name} 👉 {e.TotalTime}"));
    //    //    var sendMessage = await PRTelegramBot.Helpers.Message.Send(client, update, msg.ToString());
    //    //}


    //    //string msg1 = "Меню";
    //    ////Создаем настройки сообщения
    //    //var option = new OptionMessage();
    //    ////Создаем список для меню
    //    //var menuList = new List<KeyboardButton>();
    //    //foreach (var t in test)
    //    //{
    //    //    //Добавляем кнопку с текстом
    //    //    menuList.Add(new KeyboardButton(t.Name));
    //    //}


    //    ////Генерируем reply меню
    //    ////1 столбец, коллекция пунктов меню, вертикальное растягивание меню, пункт в самом низу по умолчанию
    //    //var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");
    //    ////Добавляем в настройки меню
    //    //option.MenuReplyKeyboardMarkup = menu;
    //    //await PRTelegramBot.Helpers.Message.Send(client, update, msg1, option);








    //    string msg = "Выберите устройство";
    //    List<IInlineContent> menu = new List<IInlineContent>();

    //    menu.Add(new InlineCallback("<b>Хронос</b>", CustomTHeader.Chronos));
    //    menu.Add(new InlineCallback("Курилка № 1", CustomTHeader.Kurilka1));
    //    menu.Add(new InlineCallback("Курилка № 2", CustomTHeader.Kurilka2));
    //    menu.Add(new InlineCallback("Гальваника", CustomTHeader.Galvanika));

    //    //Генерация меню в 1 столбец
    //    var menuItems = MenuGenerator.InlineKeyboard(1, menu);

    //    var options = new OptionMessage();
    //    //options.ParseMode = ParseMode.Html;
    //    options.MenuInlineKeyboardMarkup = menuItems;


    //    ////Регистрация обработчика для последовательной обработки шагов и сохранение данных
    //    ////update.RegisterStepHandler(new StepTelegram(StepOne, new StepCache()));
    //    await PRTelegramBot.Helpers.Message.Send(client, update, msg, options);
    //}


    #region Users

    //[SlashHandler("/users")]
    //public async Task GetUser(ITelegramBotClient client, Update update)
    //{

    //    var user = await _userManager.GetUserByNameAsync("Акс");
    //    string msg = $"/{user.Id} {user.Name}";

    //    await PRTelegramBot.Helpers.Message.Send(client, update, msg);
    //}

    [SlashHandler("/userList")]
    public async Task GetUserList(ITelegramBotClient client, Update update)
    {

        var user = await _userManager.GetUserListAsync();

        var sb = new StringBuilder();
        //sb.AppendJoin("\n", user.Select(e => e.Name = $"/u{e.Id} {e.Name}  {e.LastUsedReaderName}"));
        foreach (var u in user.Where(e=>e.LastUsedReaderName is not null).GroupBy(e=>e.LastUsedReaderName))
        {
            var u1 = user.Where(e=>e.LastUsedReaderName == u.Key).ToList();
            Console.WriteLine($"-------{u.Key}-------");
            foreach (var tUser in u1)
            {
                Console.WriteLine(tUser.Name);
            }
        }

        //string msg = $"/{user.Id} {user.Name}";

        await PRTelegramBot.Helpers.Message.Send(client, update, sb.ToString());
    }

    #endregion

    #region commands

    [ReplyMenuHandler("commands")]
    public async Task GetCommands(ITelegramBotClient client, Update update)
    {

        var sb = new StringBuilder();
        sb.Append($"/userlist - список всех сотрудников\n");
        sb.Append($"/groups - список групп\n");


        await PRTelegramBot.Helpers.Message.Send(client, update, sb.ToString());
    }

    [SlashHandler("/groups")]
    public async Task GetGroups(ITelegramBotClient client, Update update)
    {

        var sb = new StringBuilder();
        var gp = await _userManager.GetGroupsListAsync();
        sb.AppendJoin("\n", gp.Select(g => g.Name = $"/g{g.Id} {g.Name}"));


        await PRTelegramBot.Helpers.Message.Send(client, update, sb.ToString());

    }

    [SlashHandler("/u")]
    public async Task GetUserById(ITelegramBotClient client, Update update)
    {
        var x = update.Message.Text.Replace("/u", "");

        if (int.TryParse(x, out int userId))
        {
            Console.WriteLine(userId);
        }
        if (update.Message.Text.Contains("_"))
        {
            var spl = update.Message.Text.Split("_");
        }
        var sb = new StringBuilder();


        await PRTelegramBot.Helpers.Message.Send(client, update, sb.ToString());
        //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);
        await PRTelegramBot.Helpers.Message.DeleteMessage(client, update.Message.Chat.Id, update.Message.MessageId);

    }
    [SlashHandler("/g")]
    public async Task GetGroupInfo(ITelegramBotClient client, Update update)
    {
        var x = update.Message.Text.Replace("/g", "");

        if (int.TryParse(x, out int groupId))
        {
            var userList = await _userManager.GetUserListByGroupIdAsync(groupId);
            var sb = new StringBuilder();
            sb.AppendJoin("\n", userList.Select(u => u.Name));




            var optionMessage = Menu.MyPeopleKeyboard();


            await PRTelegramBot.Helpers.Message.Send(client, update, sb.ToString(), optionMessage);
            //await PRTelegramBot.Helpers.Message.Send(client, update, sb.ToString());
        }




        //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);
        await PRTelegramBot.Helpers.Message.DeleteMessage(client, update.Message.Chat.Id, update.Message.MessageId);

    }



    #endregion

    [ReplyMenuHandler("Главное меню", "/start")]
    public async Task MainMenu(ITelegramBotClient client, Update update)
    {
        //var c = new GWT.Client("http://localhost:55222", new HttpClient());

        //var res = await _client.ReportAsync(DateTimeOffset.Now.AddDays(-10), DateTimeOffset.Now, new List<int> { 141 },
        //    new List<int> { 142 });
        var msg = "Главное меню";


        await PRTelegramBot.Helpers.Message.Send(client, update, msg, Menu.MainMenuKeyboard());
        //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.MessageId);
    }

    public static DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("ru-RU", false).DateTimeFormat;

    [ReplyMenuHandler("Устройства")]
    public async Task ReportMenu(ITelegramBotClient client, Update update)
    {
        var msg = "Выберите устройство регистрации!";
        //Создаем настройки сообщения
        var option = new OptionMessage();

        //Создаем список для меню
        var menuList = new List<KeyboardButton>();

        menuList.Add(new KeyboardButton("Хронос"));
        menuList.Add(new KeyboardButton("Курилка № 1"));
        menuList.Add(new KeyboardButton("Курилка № 2"));
        menuList.Add(new KeyboardButton("Гальваника"));


        var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");

        option.MenuReplyKeyboardMarkup = menu;

        var calendarMarkup = Markup.Calendar(DateTime.Today, dtfi);
        var option1 = new OptionMessage();
        option1.MenuInlineKeyboardMarkup = calendarMarkup;


        await PRTelegramBot.Helpers.Message.Send(client, update.GetChatId(), $"Выберите дату    11111111111111:", option1);

        //await PRTelegramBot.Helpers.Message.Send(client, update, msg, option);

    }



    //[ReplyMenuHandler("Хронос")]
    //[ReplyMenuHandler("Отчет")]
    //public async Task ReportChronosMenu(ITelegramBotClient client, Update update)
    //{
    //    string msg = "Выберите период";
    //    //Создаем настройки сообщения
    //    var option = new OptionMessage();

    //    //Создаем список для меню
    //    var menuList = new List<KeyboardButton>();

    //    menuList.Add(new KeyboardButton("Сегодня\n 00:00 - 23:59"));
    //    menuList.Add(new KeyboardButton("Вчера\n 04:00 + 30 часов"));
    //    menuList.Add(new KeyboardButton("Текущая неделя"));
    //    menuList.Add(new KeyboardButton("Предыдущая неделя"));
    //    menuList.Add(new KeyboardButton("Текущий месяц"));
    //    menuList.Add(new KeyboardButton("Предыдущий месяц"));
    //    menuList.Add(new KeyboardButton("Другое"));



    //    var menu = MenuGenerator.ReplyKeyboard(2, menuList, true, "Главное меню");

    //    option.MenuReplyKeyboardMarkup = menu;



    //    await PRTelegramBot.Helpers.Message.Send(client, update, msg, option);
    //    //await PRTelegramBot.Helpers.Message.DeleteChat(client, update.Message.Chat.Id, update.Message.Code);

    //}

    [ReplyMenuHandler("👷‍♂️Кто на работе", "/whoshere","/workerstoday")] //Workers Today - рабочие сегодня
    public async Task Whoshere(ITelegramBotClient client, Update update)
    {
        var workersToday = await _report.GetWorkersTodayAsync();
        var nightShift = await _report.GetNightShiftAsync();
        if (workersToday.Count == 0)
        {
            await PRTelegramBot.Helpers.Message.Send(client, update, "Информация за указанный период отсутствует 🤷‍♂️");
            return;
        }

        var msg = new StringBuilder();
        var m = msg.AppendJoin("\n", workersToday.Select(e => $"☀ [{e.Group.Id}] {e.Name}"));

        if (nightShift.Count > 0)
        {
            m = m.Append($"\n").Append("\n");
            m = m.AppendJoin("\n", nightShift.Select(e => $"🌒 [{e.Group.Id}] {e.Name}"));
        }

        //await PRTelegramBot.Helpers.Message.Send(client, update, m.ToString(),Menu.MainMenuKeyboard());
        await PRTelegramBot.Helpers.Message.Send(client, update, m.ToString(), Menu.MyPeopleKeyboard());
    }


    #region Tracking
    //[ReplyMenuHandler("Трекинг")]
    public async Task Tracking(ITelegramBotClient client, Update update)
    {

        //var startDateTime = DateTime.Parse($"{DateTime.Now:yyyy/MM/dd} 04:00");
        //var endDateTime = DateTime.Parse($"{DateTime.Now:yyyy/MM/dd} 23:59");
        var startTime = TimeSpan.Parse("06:00");
        var endTime = TimeSpan.Parse("10:00");
        Stopwatch stopwatch = new Stopwatch();
        //засекаем время начала операции



        var now = DateTime.Now;
        // Отчетная дата - начало каждого месяца. Это первое число плюс время
        var startDateTime = new DateTime(now.Year, now.Month, 1).AddMonths(-4).Add(startTime);
        // Конечная дата, это начало следующего месяца. 
        var endDateTime = new DateTime(now.Year, now.Month, 1).Add(endTime);
        await PRTelegramBot.Helpers.Message.Send(client, update, "Обработка запроса...");
        stopwatch.Start();
        var userList = await _report.GetUsersAsync(startDateTime, endDateTime);
        stopwatch.Stop();

        //смотрим сколько миллисекунд было затрачено на выполнение
        Console.WriteLine(stopwatch.ElapsedMilliseconds);

        var sendMessage = await PRTelegramBot.Helpers.Message.Send(client, update, "Готовим документ к отправке");

        var workbook = new Workbook();
        workbook.Version = ExcelVersion.Version2016;

        workbook.Worksheets.Clear();
        var worksheet = workbook.Worksheets.Add("Отчет о событиях");

        var dataTable = new DataTable();
        dataTable.Columns.Add("Ф.И.О", typeof(string));
        dataTable.Columns.Add("Дата", typeof(string));
        dataTable.Columns.Add("Устройство", typeof(string));
        dataTable.Columns.Add("Событие", typeof(string));







        var groups = userList.GroupBy(e => e.Group.Name);


        foreach (var group in groups)
        {
            Console.WriteLine(group.Key);

            foreach (var user in group)
            {
                //Console.WriteLine(user.Name);
                foreach (var evt in user.Events)
                {
                    //Console.WriteLine($"{group.Key} {user.Name} {evt.DateTime} {evt.Reader.Name} {evt.Message.FullName}");
                }
            }
        }




        foreach (var user in userList.OrderBy(u => u.Name))
        {

            foreach (var evt in user.Events!.OrderBy(e => e.DateTime))
            {
                var dataRow = dataTable.NewRow();
                dataRow[0] = user.Name;
                dataRow[1] = evt.DateTime.ToString("G");
                dataRow[2] = evt.Reader.Name;
                //dataRow[3] = evt.Message.Text;
                dataRow[3] = evt.Message;

                dataTable.Rows.Add(dataRow);
            }

        }


        worksheet.InsertDataTable(dataTable, true, 1, 1, true);
        worksheet.AllocatedRange.AutoFitColumns();
        var path = $"\\\\10.65.68.210\\Export2\\Tmp\\Chronos\\tracking.xlsx";
        workbook.SaveToFile(path, ExcelVersion.Version2016);
        await PRTelegramBot.Helpers.Message.SendFile(client, update.GetChatId(), "Tracking", path);

    }


    #endregion



    //[ReplyMenuHandler("Другое")]
    public async Task OtherPeriod(ITelegramBotClient client, Update update)
    {

        update.RegisterStepHandler(new StepTelegram(PeriodStep, new CreateReportCache()));



        await PRTelegramBot.Helpers.Message.Send(client, update, "Укажите пожалуйста период в формате ДДММГГГГ - ДДММГГГГ");
    }


    public async Task PeriodStep(ITelegramBotClient client, Update update)
    {
        if (update.Message != null)
        {
            var periodText = update.Message.Text;
            var date = periodText.TrimEnd().Replace(" ", "").Split("-");
            DateTime start;
            DateTime end;
            if (date.Length == 2 && DateTime.TryParse(date[0], out start) && DateTime.TryParse(date[1], out end))
            {
                if (end > start) Console.WriteLine(date.ToString());

                var res = await _report.GetReportByReaders(start.AddHours(6), end.AddHours(10), new List<int> { 141 }, new List<int>() { 142 });
                var usrList = res.Workers.Where(e => e.WorkTimes.Count > 0).ToList();
                var test = usrList.GroupBy(e => e.Group.Name).Select(e => new Group() { Name = e.Key, Workers = e.ToList() })
                    .ToList();


                foreach (var t in test)
                {
                    var msg = new StringBuilder();
                    msg.Append($"<b>#⃣ {t.Name} #⃣</b>\n\r");
                    msg.AppendJoin("\n", t.Workers.Select(e => e.Name = $"🙎‍♂️ {e.Name} 👉 {e.TotalTime}").OrderBy(e => e));


                    //var calendarMarkup = Markup.Calendar(DateTime.Today, dtfi);

                    //var option = new OptionMessage();
                    //option.MenuInlineKeyboardMarkup = calendarMarkup;
                    //await PRTelegramBot.Helpers.Message.Send(client, update.GetChatId(), msg.ToString(), option);



                    //Console.WriteLine(stopwatch.ElapsedMilliseconds);
                    var sendMessage = PRTelegramBot.Helpers.Message.Send(client, update, msg.ToString());
                }

            }



        }


    }

    [ReplyMenuHandler("Другое")]
    public async Task Test(ITelegramBotClient client, Update update)
    {
        await PRTelegramBot.Helpers.Message.Send(client, update, "Обработка запроса...");
        var startTime = TimeSpan.Parse("06:00");
        var endTime = TimeSpan.Parse("10:00");



        var now = DateTime.Now;
        var startDateTime = new DateTime(now.Year, now.Month, 1).AddMonths(-1).Add(startTime);
        var endDateTime = new DateTime(now.Year, now.Month, 1).Add(endTime);

        await PRTelegramBot.Helpers.Message.Send(client, update, "Обращение к базе данных");
        Stopwatch stopwatch = new Stopwatch();
        //засекаем время начала операции
        stopwatch.Start();
        var res = await _report.GetReportByReaders(startDateTime, endDateTime, new List<int> { 141, 87, 110 }, new List<int>() { 142, 88, 109 });
        stopwatch.Stop();
        var usrList = res.Workers.Where(e => e.WorkTimes.Count > 0).ToList();
        var test = usrList.GroupBy(e => e.Group.Name)
            .Select(e => new Group() { Name = e.Key, Workers = e.ToList() })
            .OrderBy(e => e.Name)
            .ToList();


        //смотрим сколько миллисекунд было затрачено на выполнение
        Console.WriteLine(stopwatch.ElapsedMilliseconds);
        var sendMessage = await PRTelegramBot.Helpers.Message.Send(client, update, "Подготовка документа к отправке");

        using var workbook = new Workbook();

        workbook.Version = ExcelVersion.Version2016;

        workbook.Worksheets.Clear();
        //var worksheet = workbook.Worksheets.Add("Отчет за месяц");
        //CellStyle style = workbook.Styles.Add("newStyle");

        //var dataTable = new DataTable();
        //dataTable.Columns.Add("Ф.И.О", typeof(string));
        //dataTable.Columns.Add("Дата/время входа", typeof(string));
        //dataTable.Columns.Add("Устройство входа", typeof(string));
        //dataTable.Columns.Add("Дата/время выхода", typeof(string));
        //dataTable.Columns.Add("Устройство выхода", typeof(string));
        //dataTable.Columns.Add("Время работы", typeof(string));

        var quickReportWorksheet = workbook.Worksheets.Add("Краткий отчет");
        using var quickReportTable = new DataTable();


        quickReportTable.Columns.Add("Ф.И.О.", typeof(string));
        quickReportTable.Columns.Add("Итого", typeof(string));
        quickReportTable.Columns.Add("Участок", typeof(string));

        foreach (var group in test.OrderBy(e => e.Name).OrderByDescending(e => e.Workers.Count))
        {
            var detailsReportWorksheet = workbook.Worksheets.Add(group.Name);

            var detailsTable = new DataTable();
            detailsTable.Columns.Add("Ф.И.О", typeof(string));
            detailsTable.Columns.Add("Дата/время входа", typeof(string));
            detailsTable.Columns.Add("Устройство входа", typeof(string));
            detailsTable.Columns.Add("Дата/время выхода", typeof(string));
            detailsTable.Columns.Add("Устройство выхода", typeof(string));
            detailsTable.Columns.Add("Время работы", typeof(string));

            foreach (var user in group.Workers.OrderBy(e => e.Name))
            {

                DataRow quickReportRow = quickReportTable.NewRow();
                quickReportRow[0] = user.FullName;
                quickReportRow[1] = user.TotalTime;
                quickReportRow[2] = user.Group!.Name;

                quickReportTable.Rows.Add(quickReportRow);

                foreach (var wt in user.WorkTimes)
                {
                    DataRow dataRow = detailsTable.NewRow();
                    dataRow[0] = user.Name;
                    dataRow[1] = wt.EntryTime.ToString("G");
                    dataRow[2] = wt.FirstReader;
                    dataRow[3] = wt.ExitTime.ToString("G");
                    dataRow[4] = wt.LastReader;
                    dataRow[5] = wt.Tot;
                    detailsTable.Rows.Add(dataRow);

                }
                detailsTable.Rows.Add(detailsTable.NewRow());

            }

            quickReportWorksheet.InsertDataTable(quickReportTable, true, 1, 1, true);

            quickReportTable.Rows.Add(quickReportTable.NewRow());

            detailsTable.Rows.Add(detailsTable.NewRow());

            detailsReportWorksheet.InsertDataTable(detailsTable, true, 1, 1, true);
            quickReportWorksheet.AllocatedRange.AutoFitColumns();

            var filters = quickReportWorksheet.AutoFilters;
            filters.Range = quickReportWorksheet.Range[1, 3, quickReportWorksheet.LastRow, 3];

            filters.Filter();



            detailsReportWorksheet.AllocatedRange.AutoFitColumns();

        }
        quickReportWorksheet.Range[1, 2, quickReportWorksheet.LastRow, 2].Style.HorizontalAlignment = HorizontalAlignType.Center;
        quickReportWorksheet.Range[1, 2, quickReportWorksheet.LastRow, 2].Style.Font.IsBold = true;
        quickReportWorksheet.Range[1, 1, 1, 3].Style.Font.IsBold = true;
        var start = res.Start.ToString("d");
        var end = res.End.ToString("d");
        //workbook.SaveToFile($"Report_{start}_{end}.xlsx", ExcelVersion.Version2016);



        using var ms = new MemoryStream();
        workbook.SaveToStream(ms, FileFormat.Version2016);
        ms.Seek(0, SeekOrigin.Begin);

        //var file = InputFile.FromStream(ms, $"fullReport {start}-{end}.xlsx");
        var file = InputFile.FromStream(ms, $"отчет за {res.Start:Y}.xlsx");

        var send = await client.SendDocumentAsync(update.GetChatId(), file);

        //stream.Close();
        //workbook.SaveToStream();
        //await PRTelegramBot.Helpers.Message.SendFile(client, update.GetChatId(), "Отчет", $"Report_{start}-{end}.xlsx");

    }

    #region Курилки

    [ReplyMenuHandler("Курилка № 1")]
    public async Task Kurilka1Report(ITelegramBotClient client, Update update)
    {

        var res = await _report.GetReportByReaders(DateTime.Parse("2024-01-31 04:00:00.00"),
            DateTime.Now, new List<int> { 113 }, new List<int>() { 114 });
        var usrList = res.Workers.Where(e => e.WorkTimes.Count > 0).ToList();
        var test = usrList.GroupBy(e => e.Group.Name).Select(e => new Group() { Name = e.Key, Workers = e.ToList() }).ToList();

        foreach (var t in test)
        {
            var msg = new StringBuilder();
            msg.Append($"<b>#⃣ {t.Name} #⃣</b>\n\r");
            msg.AppendJoin("\n", t.Workers.Select(e => e.Name = $"🙎‍♂️ {e.Name} 👉 {e.TotalTime}"));
            var sendMessage = await PRTelegramBot.Helpers.Message.Send(client, update, msg.ToString());
        }
    }

    #endregion




    #region Отчет за период

    [ReplyMenuHandler("Сегодня")]
    public async Task Today(ITelegramBotClient client, Update update)
    {
        //client.InvokeCommonLog("Отчет за сегодня");
        _logger.LogInformation($"{update.GetInfoUser()} запросил отчет за сегодня");
        // offset - 6:00 смещение
        var startDayTime = DateTime.Parse($"{DateTime.Now:yyyy/MM/dd} 06:00");
        var endDayTime = DateTime.Parse($"{DateTime.Now:yyyy/MM/dd} 23:59:59");

        var reportType = update.GetCacheData<CreateReportCache>().Type;

        switch (reportType)
        {
            case ReportType.Quick:
                await SendReportMessageAsync(client, update, startDayTime, endDayTime);
                break;
            case ReportType.Detailed:
                await SendReportDocumentAsync(client, update, startDayTime, endDayTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }


    [ReplyMenuHandler("Вчера")]
    public async Task Yesterday(ITelegramBotClient client, Update update)
    {
        var startDateTime = DateTime.Today.AddDays(-1).AddHours(6); // вчера с 6 утра
        var endDayTime = DateTime.Today.AddHours(10); // сегодня до 10 утра

        await SendReportAsync(client, update, startDateTime, endDayTime);

    }

    // Отчет за текущий месяц
    [ReplyMenuHandler("Текущий месяц")]
    public async Task CurrentMonthReport(ITelegramBotClient client, Update update)
    {
        var startTime = TimeSpan.Parse("06:00");
        var now = DateTime.Now;
        // тачало текущего месяца
        var startDateTime = new DateTime(now.Year, now.Month, 1).Add(startTime);
        var endDayTime = DateTime.Now;

        await SendReportAsync(client, update, startDateTime, endDayTime);
 

    }

    // Отчет за предыдущий месяц
    [ReplyMenuHandler("Предыдущий месяц")]
    public async Task LastMonthReport(ITelegramBotClient client, Update update)
    {
        var startTime = TimeSpan.Parse("06:00");
        var endTime = TimeSpan.Parse("10:00");

        var now = DateTime.Now;
        // Отчетная дата - начало каждого месяца. Это первое число плюс время
        var startDateTime = new DateTime(now.Year, now.Month, 1).AddMonths(-1).Add(startTime);
        // Конечная дата, это начало следующего месяца. 
        var endDayTime = new DateTime(now.Year, now.Month, 1).Add(endTime);

        await SendReportAsync(client, update, startDateTime, endDayTime);
    }


    [ReplyMenuHandler("1-я пол. месяца")]
    public async Task ReportMonthPartOne(ITelegramBotClient client, Update update)
    {

        await PRTelegramBot.Helpers.Message.Send(client, update, "Обработка запроса...");
        var startTime = TimeSpan.Parse("06:00");
        var endTime = TimeSpan.Parse("10:00");

        var now = DateTime.Now;

        var startDateTime = new DateTime(now.Year, now.Month, 1).Add(startTime);
        var endDayTime = new DateTime(now.Year, now.Month, 16).Add(endTime);

        
        await SendReportAsync(client, update, startDateTime, endDayTime);


    }

    [ReplyMenuHandler("2-я пол. месяца")]
    public async Task ReportMonthPartTwo(ITelegramBotClient client, Update update)
    {
        await PRTelegramBot.Helpers.Message.Send(client, update, "Обработка запроса...");
        var startTime = TimeSpan.Parse("06:00");
        var endTime = TimeSpan.Parse("10:00");

        var now = DateTime.Now;

        var startDateTime = new DateTime(now.Year, now.Month, 16).Add(startTime);
        var endDayTime = new DateTime(now.Year, now.Month, 1).Add(endTime);


        // если с начало месяца не прошло 15 дней, то отчет должен быть сформирован за вторую половину предыдущего месяца
        if (now.Day <= 15)
        {
            startDateTime = startDateTime.AddMonths(-1);
        }
        // если прошло больше 15, то отчет формируется за текущий месяц. 
        if (now.Day > 15)
        {
            endDayTime = endDayTime.AddMonths(1);
        }

        await SendReportAsync(client, update, startDateTime, endDayTime);

    }

    #endregion

    private async Task SendReportAsync(ITelegramBotClient client, Update update, DateTime startDateTime, DateTime endDayTime)
    {
        var reportType = update.GetCacheData<CreateReportCache>().Type;
        switch (reportType)
        {
            case ReportType.Quick:
                await SendReportMessageAsync(client, update, startDateTime, endDayTime);
                break;
            case ReportType.Detailed:
                await SendReportDocumentAsync(client, update, startDateTime, endDayTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private async Task SendReportMessageAsync(ITelegramBotClient client, Update update, DateTime startDayTime, DateTime endDayTime)
    {
        var report = await _report.GetReportByReaders(startDayTime, endDayTime, new List<int> { 141 }, new List<int>() { 142 });

        var usrList = report.Workers.Where(e => e.WorkTimes.Count > 0).ToList();
        var groups = usrList.GroupBy(e => e.Group.Name).Select(e => new Group() { Name = e.Key, Workers = e.ToList() })
            .ToList();


        if (groups.Count == 0) await PRTelegramBot.Helpers.Message.Send(client, update, "Информация за указанный период отсутствует 🤷‍♂️");

        foreach (var group in groups)
        {
            var msg = new StringBuilder();
            msg.Append($"<b>#⃣ {group.Name} #⃣</b>\n\r");
            msg.AppendJoin("\n", group.Workers.Select(e => e.Name = $"🙎‍♂️ {e.Name} 👉 {e.TotalTime}").OrderBy(e => e));
            var sendMessage = await PRTelegramBot.Helpers.Message.Send(client, update, msg.ToString());
        }
    }

    private async Task SendReportDocumentAsync(ITelegramBotClient client, Update update, DateTime startDayTime, DateTime endDayTime)
    {
        //await PRTelegramBot.Helpers.Message.Send(client, update, "Обращение к базе данных");
        var inputReader = new List<int> { 141 };
        var outputReader = new List<int>() { 142 };


        var report = await _report.GetReportByReaders(startDayTime, endDayTime, inputReader, outputReader);
        var workers = report.Workers.Where(e => e.WorkTimes.Count > 0).ToList();
        if (workers.IsNullOrEmpty())
        {
            var sendMessage = await PRTelegramBot.Helpers.Message.Send(client, update, "Информация за указанный период отсутствует 🤷‍♂️");
            return;
        }

        var workbook = await GenerateReportFileStreamAsync(workers);

        await PRTelegramBot.Helpers.Message.Send(client, update, $"Подготовка отчета с {startDayTime:g} по {endDayTime:g}");

        await using var ms = new MemoryStream();
        workbook.SaveToStream(ms, FileFormat.Version2016);
        ms.Seek(0, SeekOrigin.Begin);

        var file = InputFile.FromStream(ms, $"отчет c {startDayTime:d} - {endDayTime:d}.xlsx");
        var send = await client.SendDocumentAsync(update.GetChatId(), file);
        workbook.Dispose();
    }

    public async Task<Workbook> GenerateReportFileStreamAsync(List<Application.DTOs.Worker> usrList)
    {
        return await Task.Run(() =>
        {
            var groups = usrList.GroupBy(e => e.Group.Name)
            .Select(e => new Group() { Name = e.Key, Workers = e.ToList() })
            .OrderBy(e => e.Name)
            .ToList();


            //using var workbook = new Workbook();
            var workbook = new Workbook
            {
                Version = ExcelVersion.Version2016
            };

            workbook.Worksheets.Clear();


            var quickReportWorksheet = workbook.Worksheets.Add("Краткий отчет");
            using var quickReportTable = new DataTable();


            quickReportTable.Columns.Add("Ф.И.О.", typeof(string));
            quickReportTable.Columns.Add("Итого", typeof(string));
            quickReportTable.Columns.Add("Участок", typeof(string));
            quickReportTable.Columns.Add("Договор", typeof(string));

            foreach (var group in groups.OrderBy(e => e.Name).OrderByDescending(e => e.Workers.Count))
            {
                var detailsReportWorksheet = workbook.Worksheets.Add(group.Name);

                var detailsTable = new DataTable();
                detailsTable.Columns.Add("Ф.И.О", typeof(string));
                detailsTable.Columns.Add("Дата/время входа", typeof(string));
                detailsTable.Columns.Add("Устройство входа", typeof(string));
                detailsTable.Columns.Add("Дата/время выхода", typeof(string));
                detailsTable.Columns.Add("Устройство выхода", typeof(string));
                detailsTable.Columns.Add("Время работы", typeof(string));

                foreach (var user in group.Workers.OrderBy(e => e.Name))
                {

                    DataRow quickReportRow = quickReportTable.NewRow();
                    quickReportRow[0] = user.FullName;
                    quickReportRow[1] = user.TotalTime;
                    quickReportRow[2] = user.Group!.Name;
                    quickReportRow[3] = user.ContractInfo;
                    quickReportTable.Rows.Add(quickReportRow);

                    foreach (var wt in user.WorkTimes)
                    {
                        DataRow dataRow = detailsTable.NewRow();
                        dataRow[0] = user.Name;
                        dataRow[1] = wt.EntryTime.ToString("G");
                        dataRow[2] = wt.FirstReader;
                        dataRow[3] = wt.ExitTime.ToString("G");
                        dataRow[4] = wt.LastReader;
                        dataRow[5] = wt.Tot;
                        detailsTable.Rows.Add(dataRow);

                    }

                    detailsTable.Rows.Add(detailsTable.NewRow());

                }

                quickReportWorksheet.InsertDataTable(quickReportTable, true, 1, 1, true);

                quickReportTable.Rows.Add(quickReportTable.NewRow());

                detailsTable.Rows.Add(detailsTable.NewRow());

                detailsReportWorksheet.InsertDataTable(detailsTable, true, 1, 1, true);
                quickReportWorksheet.AllocatedRange.AutoFitColumns();

                //var filters = quickReportWorksheet.AutoFilters;
                //filters.Range = quickReportWorksheet.Range[1, 3, quickReportWorksheet.LastRow, 3];

                //filters.Filter();

                var filters = quickReportWorksheet.AutoFilters;
                filters.Range = quickReportWorksheet.Range[1, 3, quickReportWorksheet.LastRow, 4];

                filters.Filter();

                detailsReportWorksheet.AllocatedRange.AutoFitColumns();

            }

            quickReportWorksheet.Range[1, 2, quickReportWorksheet.LastRow, 2].Style.HorizontalAlignment =
                HorizontalAlignType.Center;
            quickReportWorksheet.Range[1, 4, quickReportWorksheet.LastRow, 4].Style.HorizontalAlignment =
                HorizontalAlignType.Center;
            quickReportWorksheet.Range[1, 2, quickReportWorksheet.LastRow, 2].Style.Font.IsBold = true;
            // заголовок таблицы
            quickReportWorksheet.Range[1, 1, 1, 4].Style.Font.IsBold = true;
            return workbook;
        });
    }
}
