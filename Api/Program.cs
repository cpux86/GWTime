using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Api.BotControllers.Dialog;
using Application;
using Application.Services;
using Microsoft.OpenApi.Extensions;
using NLog;
using NLog.Extensions.Logging;
//using NLog.Web;
using Persistence;
using PRTelegramBot.Configs;
using PRTelegramBot.Core;
using PRTelegramBot.Extensions;
using RabbitMQ.Client;
using Telegram.Bot;
using Telegram.Bot.Types;
using Helpers = PRTelegramBot.Helpers;
using Update = Telegram.Bot.Types.Update;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddPersistence();
builder.Services.AddApplication();
builder.Services.AddBotHandlers();
//builder.Logging.ClearProviders();
builder.Logging.AddNLog();
//builder.Host.UseNLog();

//builder.Host.ConfigureLogging(opt =>
//{
//    opt.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
//    opt.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
//});

//builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


//Создание и запуск бота
var botInstance = new PRBot(new TelegramConfig
    {
        //Token = "6679926909:AAFQ8OSxR3GtYFRXepqfrX7dvkwkczVAgoI", // Production
        Token = "6581396259:AAHak1OPEZiUJ5R0bSJDb3GZQe9MnSuuznc", // Demo
        ClearUpdatesOnStart = true,
        BotId = 0
    },

    app.Services.GetService<IServiceProvider>()
);


await botInstance.Start();
botInstance.Handler.Router.OnMissingCommand += RouterOnMissingCommand;
//botInstance.Handler.OnPreUpdate += Handler_OnPreUpdate;

//static async Task<PRTelegramBot.Models.Enums.ResultUpdate> Handler_OnPreUpdate(ITelegramBotClient arg1, Update update)
//{
//    Console.WriteLine();
//}

botInstance.OnLogCommon += BotInstanceOnLogCommon;
await botInstance.botClient.SetMyCommandsAsync(new List<BotCommand>()
{
    new BotCommand()
    {
        Command = "tracking",
        Description = "Отслеживание"
    },
    new BotCommand()
    {
        Command = "whoshere",
        Description = "На работе сегодня"
    }
});

 void BotInstanceOnLogCommon(string msg, Enum typeEvent, ConsoleColor color)
{
    //Console.ForegroundColor = color;
    //Console.WriteLine(msg);

    if (typeEvent == null)
    {
       logger.LogInformation(msg);
    }
    
#if DEBUG
    Console.WriteLine("Debug version");
#endif

}

static async Task RouterOnMissingCommand(ITelegramBotClient client, Update update)
{
    update.ClearStepUserHandler();
    await Helpers.Message.Send(client, update.GetChatId(), "не верный запрос /start");

}

app.Run();

