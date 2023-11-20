using Application;
using Persistence;
using System;
using System.Diagnostics;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistence();
builder.Services.AddApplication();
//builder.Services.AddHostedService<EventLoggerMonitor>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();

//builder.Services.AddHostedService<EventLoggerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//CultureInfo.CurrentCulture = new CultureInfo("ru-RU");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();

