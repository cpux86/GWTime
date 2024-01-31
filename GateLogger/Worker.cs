using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using GateLogger.Services;
using NetCoreServer;
using System.Text;
using System.Text.RegularExpressions;
using GateLogger.Infrastructure;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using GateLogger.Services.StartEvents;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    Console.WriteLine(1);
            //}
            
            

           

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
                    var client = new GateTcpClient(ip);
                    client.ConnectAsync();

                }

            GateTcpClient.NewEvent += NewEventHandler;
            await Task.CompletedTask;

        }

        
        private static void NewEventHandler(object? sender, EventResponse e)
        {
            if (e.UserId == 0)
            {
                Console.WriteLine($"Log {e.message}");
                return;
            }

            using var db = new EventsDbContext();

            // тип события
            var message = db.Messages.FirstOrDefault(m => m.Id == e.EventCode) ??
                          new Message { Id = (byte)e.EventCode, Text = e.message };
            message.Text = e.message;


            var userName = Wiegand26Regex().Replace(e.UserName, "").Trim();
            // извлекаем номер ключа
            var key = Wiegand26Regex().Match(e.UserName).Value;

            var reader = db.Readers.FirstOrDefault(r => r.Id == (short)e.ReaderId) ?? new Reader { Id = (short)e.ReaderId, Name = e.ReaderName };
            reader.Name = e.ReaderName;
            // если пользователю не присвоена группа, то помещаем его в группу "Без группы"
            if (e.Group == string.Empty)
            {
                e.Group = "Без группы";
            }

            var group = db.UserGroups.FirstOrDefault(g => g.Name == e.Group) ?? new UserGroup{Name = e.Group};
            if (group.Id == 0)
            {
                db.UserGroups.Add(group);
            }

            var user = db.Users.FirstOrDefault(u => u.Id == e.UserId) ?? new User { Id = e.UserId, FullName = e.FullName, Name = userName };

            user.Name = userName;
            user.FullName = e.FullName;
            user.UserGroup = group;
            //user.Key = key;


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
                db.SaveChanges();

                Console.WriteLine($"{gateEvent.DateTime:G} {user.Name} {e.message} {e.ReaderName}");
                //_logger.LogInformation(message: e.message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.InnerException.Message);
            }
        }

        [GeneratedRegex("^[0-9]{3}\\/[0-9]{5}")]
        private static partial Regex Wiegand26Regex();

    }






}