using Domain;
using GateLogger.Infrastructure;
using GateLogger.Services;
using GateLogger.Services.StartEvents;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Group = Domain.Group;


namespace GateLogger;




public partial class Worker : BackgroundService
{




    private static ILogger<Worker> _logger;

    //private readonly EventsDbContext _dbContext;
    private static IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IMemoryCache cache, IConfiguration configuration)
    {
        _logger = logger;
        _cache = cache;
        _configuration = configuration;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        GateTcpClient.NewEvent += NewEventHandler;
        


        var serverValue = _configuration.GetSection("GateServer").Value;

        var addressList = serverValue?.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (addressList != null)
            foreach (var address in addressList)
            {
                var endpoint = IPEndPoint.Parse(address);
                if (endpoint.Port == 0) endpoint.Port = 1917;
                var ip = endpoint.Address.ToString();
                var port = endpoint.Port;
                //new GateTcpClient(ip, 1234).ConnectAsync();
                new GateTcpClient(ip, 1917)
                {
                    OptionKeepAlive = true,
                    OptionTcpKeepAliveTime = 30
                }.ConnectAsync();


                //var client = new GateTcpClient(ip)
                //{
                //    OptionKeepAlive = true,
                //    OptionTcpKeepAliveTime = 30
                //};
                //client.ConnectAsync();
                

                //using var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


                //var  args = new SocketAsyncEventArgs();


                //sock.ConnectAsync(args);



                //var socket = args.ConnectSocket;
                //if (socket != null)
                //    await socket.ConnectAsync(IPAddress.Parse("10.65.69.99"), 1917, CancellationToken.None);
                //if (socket.Connected)
                //{
                //    Console.WriteLine("Connected ");
                //}






            }
        while (true)
        {
            var r = await GateTcpClient.ReadMessage();
            Console.WriteLine($"пользователь : {r.FullName}");
        }


        await Task.CompletedTask;
    }


    private static void NewEventHandler(EventResponse e)
    {
        if (e.UserId == 0)
        {
            Console.WriteLine($"Log {e.Message} {e.ReaderName}");
            return;
        }

        using var db = new EventsDbContext();


        //var eUsers = db.Users.ToList();
        //foreach (var user1 in eUsers)
        //{
        //    user1.Key = "999/99999";
        //    user1.LastEventMessage = string.Empty;
        //    user1.LastUsedKeyDate = DateTime.MinValue;
        //    user1.LastUsedReaderName = string.Empty;
        //}

        //db.SaveChanges();

        var userName = Wiegand26Regex().Replace(e.UserName, "").Trim();
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
        //User user;
        //if (!_cache.TryGetValue(e.UserId, out user))
        //{
        //    user = db.Users.FirstOrDefault(u => u.Id == e.UserId) ?? new User { Id = e.UserId, FullName = e.FullName, Name = userName };
        //    if (user != null)
        //    {
        //        _cache.Set(user.Id, user,
        //            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        //    }
        //}

        

        //var users = db.Users.Where(u => u.Id == e.UserId || u.Key == key).ToList() ?? new List<User>()
        //{
        //    new User { Id = e.UserId, FullName = e.FullName, Name = userName }
        //};

        var users = db.Users.Where(u => u.Id == e.UserId || u.Key == key).ToList();

        foreach (var u in users.Where(u => u.Id != e.UserId))
        {
            u.Key = string.Empty;
        }



        user.Name = userName;
        user.FullName = e.FullName;
        user.Group = group;
        user.Key = key;

        var dateTime = DateTime.Parse(e.DateTime);
        // если пришло новое событие или проходов у user еще не было, то обновляем данные о последнем проходе проходе
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
            //await db.SaveChangesAsync(CancellationToken.None);
            db.SaveChanges();
            Console.WriteLine($"{evt.DateTime:G} {user.Name} {e.Message} {e.ReaderName}");

            //_logger.LogInformation(message: e.message);
        }

        catch (Exception exception)
        {
            Console.WriteLine(exception.InnerException!.Message);
        }
    }

    [GeneratedRegex("^[0-9]{3}\\/[0-9]{5}")]
    private static partial Regex Wiegand26Regex();

}