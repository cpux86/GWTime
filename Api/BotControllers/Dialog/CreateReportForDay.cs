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
    //private readonly GWT.Client _client;
    public CreateReportForDay(IReportService report, IUserManager userManager, ILogger<CreateReportForDay> logger)
    {
        _report = report;
        _userManager = userManager;
        _logger = logger;
        //_client = new GWT.Client("http://localhost:55222", new HttpClient());
    }

  

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
        var msg = "Главное меню";
        await PRTelegramBot.Helpers.Message.Send(client, update, msg, Menu.MainMenuKeyboard());
    }

    public static DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("ru-RU", false).DateTimeFormat;




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

        await PRTelegramBot.Helpers.Message.Send(client, update, m.ToString(), Menu.MyPeopleKeyboard());
    }


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
        var report = await _report.GetReportByReaders(startDayTime, endDayTime, new List<int> { 141, 87}, new List<int>() { 142, 88 });

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
        var inputReader = new List<int> {
            141,
            87,
            //105
        };
        var outputReader = new List<int>() {
            142,
            88,
            //106
        };


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
