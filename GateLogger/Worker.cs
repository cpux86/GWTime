using System.Globalization;
using Domain;
using GateLogger.Infrastructure;
using GateLogger.Services;
using GateLogger.Services.StartEvents;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Group = Domain.Group;


namespace GateLogger;

public partial class Worker : BackgroundService
{
    private static ILogger<Worker> _logger;
    private static IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serverValue = _configuration.GetSection("GateServer").Value;
        if (serverValue.IsNullOrEmpty()) serverValue = "127.0.0.1:1917";
        
        var addressList = serverValue?.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (addressList != null)
            foreach (var address in addressList)
            {
                var endpoint = IPEndPoint.Parse(address);
                if (endpoint.Port == 0) endpoint.Port = 1917;
                var ip = endpoint.Address.ToString();
                var port = endpoint.Port;
                var client = new GateTcpClient(ip, port, _logger)
                {
                    OptionKeepAlive = true,
                    OptionTcpKeepAliveTime = 30
                };
                client.ConnectAsync();
            }

        while (!stoppingToken.IsCancellationRequested)
        {
            var evt = await GateTcpClient.ReadMessageAsync(stoppingToken);
            NewEventHandler(evt);
            Console.WriteLine($"Новое сообщение: {evt.FullName}");
        }
        //await Task.CompletedTask;

    }

    // обработчик ответа сервера
    private static void NewEventHandler(EventResponse e)
    {

        if (e.UserId == 0)
        {
            Console.WriteLine($"Log {e.Message} {e.ReaderName}");
            return;
        }

        using var db = new EventsDbContext();


        var userName = Wiegand26Regex().Replace(e.UserName, string.Empty).Trim();
        // извлекаем номер ключа
        var key = Wiegand26Regex().Match(e.UserName).Value;

        var reader = db.Readers
            .FirstOrDefault(r => r.Id == e.ReaderId) ?? new Reader { Id = e.ReaderId, Name = e.ReaderName };
        reader.Name = e.ReaderName;
        // если пользователю не присвоена группа, то помещаем его в группу "Без группы"
        if (e.Group.IsNullOrEmpty())
        {
            e.Group = "Без группы";
        }


        var group = db.Groups.FirstOrDefault(g => g.Name == e.Group) ?? new Group { Name = e.Group };
        if (group.Id == 0)
        {
            db.Groups.Add(group);
        }


        var user = db.Users.FirstOrDefault(u => u.Id == e.UserId) ?? new User { Id = e.UserId, FullName = e.FullName, Name = userName };


        var users = db.Users.Where(u => u.Id == e.UserId || u.Key == key).ToList();

        //foreach (var u in users.Where(u => u.Id != e.UserId))
        //{
        //    //u.Key = string.Empty;
        //    db.Users.Remove(u);
        //}

        var oldUsers = users.Where(u => u.Id != e.UserId).ToList();
        db.Users.RemoveRange(oldUsers);

        user.Name = userName;
        user.FullName = e.FullName;
        user.Group = group;
        user.Key = key;

        var dateTime = DateTime.Parse(e.DateTime);

        // если пришло новое событие или проходов у user еще не было, то обновляем данные о последнем проходе
        if (dateTime > user.LastUsedKeyDate || user.LastUsedKeyDate is null)
        {
            user.LastUsedKeyDate = dateTime;
            user.LastUsedReaderName = e.ReaderName;
            user.LastEventMessage = e.Message;
        }

        var evt = new Event
        {
            User = user,
            Reader = reader,
            DateTime = dateTime,
            Code = e.Code

        };
        
        db.Events.Add(evt);

        try
        {
            db.SaveChanges();
        }


        catch (Exception exception)
        {
            _logger.LogInformation(exception.InnerException!.Message);
        }

    }

    [GeneratedRegex("^[0-9]{3}\\/[0-9]{5}")]
    private static partial Regex Wiegand26Regex();

}