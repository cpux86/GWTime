using Domain;
using GateLogger.Infrastructure;
using GateLogger.Services;
using GateLogger.Services.StartEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using Group = Domain.Group;

namespace GateLogger
{
    public partial class Worker : BackgroundService
    {
        private static ILogger<Worker> _logger;

        //private readonly EventsDbContext _dbContext;
        private IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IMemoryCache cache, IConfiguration configuration)
        {
            _logger = logger;
            _cache = cache;
            _configuration = configuration;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public Action<EventResponse> OnResponse { get; set; }
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
                    OnResponse = NewEventHandler;
                    var client = new GateTcpClient(ip, OnResponse)
                    {
                        OptionKeepAlive = true,
                        OptionTcpKeepAliveTime = 30
                    };
                    client.ConnectAsync();
                }
            await Task.CompletedTask;
        }


        private static void NewEventHandler(EventResponse e)
        {
            if (e.UserId == 0)
            {
                Console.WriteLine($"Log {e.Message}");
                return;
            }

            using var db = new EventsDbContext();

            // тип события
            var message = db.Messages.FirstOrDefault(m => m.Id == e.EventCode) ??
                          new Message { Id = (byte)e.EventCode, Text = e.Message };
            message.Text = e.Message;


            var userName = Wiegand26Regex().Replace(e.UserName, "").Trim();
            // извлекаем номер ключа
            var key = Wiegand26Regex().Match(e.UserName).Value;

            var reader = db.Readers
                .FirstOrDefault(r => r.Id == (short)e.ReaderId) ?? new Reader { Id = (short)e.ReaderId, Name = e.ReaderName };
            reader.Name = e.ReaderName;
            // если пользователю не присвоена группа, то помещаем его в группу "Без группы"
            if (e.Group == string.Empty)
            {
                e.Group = "Без группы";
            }

            var group = db.Groups.FirstOrDefault(g => g.Name == e.Group) ?? new Group { Name = e.Group };
            if (group.Id == 0)
            {
                db.Groups.Add(group);
            }

            var user = db.Users.FirstOrDefault(u => u.Id == e.UserId) ?? new User { Id = e.UserId, FullName = e.FullName, Name = userName };
            //var user = await db.Users.FirstOrDefaultAsync(u => u.Key == key) ?? new User { Id = e.UserId, FullName = e.FullName, Name = userName };

            user.Name = userName;
            user.FullName = e.FullName;
            user.Group = group;
            user.Key = key;


            var gateEvent = new Event
            {
                User = user,
                Reader = reader,
                DateTime = DateTime.Parse(e.DateTime),
                Message = message

            };

            db.Events.Add(gateEvent);


            try
            {
                //await db.SaveChangesAsync(CancellationToken.None);
                db.SaveChanges();
                Console.WriteLine($"{gateEvent.DateTime:G} {user.Name} {e.Message} {e.ReaderName}");

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






}