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
using GWT;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//builder.Configuration.AddEnvironmentVariables();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddBotHandlers();
builder.Services.AddResponseCompression();

//builder.Logging.ClearProviders();
builder.Logging.AddNLog();
//builder.Host.UseNLog();

//builder.Host.ConfigureLogging(opt =>
//{
//    opt.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
//    opt.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
//});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();


var app = builder.Build();

//app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();
var token = builder.Configuration.GetValue<string>("Bot:Token");// ?? "6581396259:AAHak1OPEZiUJ5R0bSJDb3GZQe9MnSuuznc";


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



//Создание и запуск бота
//var botInstance = new PRBot(new TelegramConfig
//{
//    //Token = "6679926909:AAFQ8OSxR3GtYFRXepqfrX7dvkwkczVAgoI", // Production
//    //Token = "6581396259:AAHak1OPEZiUJ5R0bSJDb3GZQe9MnSuuznc", // Demo
//    Token = token,
//    ClearUpdatesOnStart = true,
//    BotId = 0
//},

//    app.Services.GetService<IServiceProvider>()
//);

var serviceProvider = app.Services.GetService<IServiceProvider>();


var telegram = new PRBotBuilder(token)
    .SetBotId(0)
    //.AddAdmin(1111111)
    .SetClearUpdatesOnStart(true)
    .SetServiceProvider(serviceProvider)
    //.AddUserWhiteList(5545443995)
    .Build();

telegram.Events.OnMissingCommand += EventsOnMissingCommand;
telegram.Events.OnCommonLog += EventsOnCommonLog;

await telegram.Start();

Task EventsOnCommonLog(PRTelegramBot.Models.EventsArgs.CommonLogEventArgs arg)
{
    logger.LogInformation(arg.Message);
    return Task.CompletedTask;
}

static async  Task EventsOnMissingCommand(PRTelegramBot.Models.EventsArgs.BotEventArgs arg)
{
    arg.Update.ClearStepUserHandler();
    await Helpers.Message.Send(arg.BotClient, arg.Update.GetChatId(), "Что-то пошло не так... /start");

}

await telegram.botClient.SetMyCommandsAsync(new List<BotCommand>()
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
    },
    new BotCommand()
    {
        Command = "start",
        Description = "Главное меню"
    }
});

app.Run();

