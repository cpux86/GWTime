using System.Text.Json.Serialization;
using Api.BotControllers.Dialog;
using Application;
using Application.Services;
using Persistence;
using PRTelegramBot.Configs;
using PRTelegramBot.Core;
using PRTelegramBot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddPersistence();
builder.Services.AddApplication();
builder.Services.AddBotHandlers();


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





app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


//Создание и запуск бота
var botInstance = new PRBot(new TelegramConfig
    {
        Token = "6320526153:AAFLv4Y7DT1XqE6prz7Fjmt0fgW-5-yBGFo",
        ClearUpdatesOnStart = true,
        BotId = 0
    },
    
    app.Services.GetService<IServiceProvider>()
);



await botInstance.Start();

app.Run();

