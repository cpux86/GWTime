using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MyNamespace;





var client = new TelegramBotClient("6320526153:AAFLv4Y7DT1XqE6prz7Fjmt0fgW-5-yBGFo", new HttpClient());

client.StartReceiving(Update, Error);
Console.ReadLine();






async Task BotMessageReceived(ITelegramBotClient client, Message? message)
{



    var mainKeyboardMarkup = new ReplyKeyboardMarkup(new[]
        {
            new[]
            {
                new KeyboardButton("⚙ Кто на работе"),
                new KeyboardButton("Отчет за сутки"),
                //new KeyboardButton("\"Call me ☎️"),
            },
            new[]
            {
                new KeyboardButton("Геолокация"),
                new KeyboardButton("Оборудование"),
            }
        })
        { ResizeKeyboard = true };



    switch (message?.Text)
    {
        case "Отчет за сутки":
        {
            var cl = new Client("http://localhost:5222", new HttpClient());

            var res = await cl.ReportAsync(DateTimeOffset.Parse("2024-01-01 06:00:00.00"), DateTimeOffset.Now, new List<int> { 141 }, new List<int>() { 142 });
            //var res = await cl.ReportAsync(DateTimeOffset.Parse("2024-01-23 06:00:00.00"), DateTimeOffset.Now, new List<int> { 113 }, new List<int>() { 114 });
            //var res = await cl.ReportAsync(DateTimeOffset.Parse("2024-01-23 06:00:00.00"), DateTimeOffset.Now, new List<int> { 131 }, new List<int>() { 132 });

                //var res = await cl.ReportAsync(DateTimeOffset.Parse("2024-01-01 06:00:00.00"), DateTimeOffset.Now, new List<int> { 113, 131 }, new List<int>() { 114, 132 });
                //InlineKeyboardMarkup devicesInlineKeyboardButton = new(new[]
                //{
                //    new[]
                //    {
                //        InlineKeyboardButton.WithCallbackData("Хронос", "select_date"),

                //    },
                //    new[]
                //    {
                //        InlineKeyboardButton.WithCallbackData("Курилка № 1", "select_date")

                //    },
                //    new[]
                //    {
                //        InlineKeyboardButton.WithCallbackData("Курилка № 2", "select_date")
                //    },
                //    new[]
                //    {
                //        InlineKeyboardButton.WithCallbackData("Гальваника", "select_date")
                //    },

                //});
                //await client.SendTextMessageAsync(message.Chat.Id, "Выберите устройство", replyMarkup: devicesInlineKeyboardButton, parseMode: ParseMode.Html);
                //return;



                InlineKeyboardMarkup InlineKeyboard = new(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(DateTime.Now.ToString("d"), "select_date")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(DateTime.Now.AddDays(-1).ToString("d"), "date_start"),
                    InlineKeyboardButton.WithCallbackData(DateTime.Now.AddDays(1).ToString("d"), "date_end"),
                }
            });

                

            var r = string.Join("\n", res.Groups.Select(e => e.Name).ToList());

                //try
                //{
                //    await client.DeleteMessageAsync(6310947780, 222372, CancellationToken.None);
                //    var r1 = await client.SendTextMessageAsync(message.Chat.Id, "1", replyMarkup: keyboard, parseMode: ParseMode.Html);
                //    Console.WriteLine(r1.MessageId);

                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //    throw;
                //}


                //try
                //{

                //    var m = await client.SendTextMessageAsync(message.Chat.Id, "1",
                //        replyMarkup: new ReplyKeyboardMarkup(new[]
                //        {
                //            new[]
                //            {
                //                new KeyboardButton("⚙ Кто на работе"),
                //                new KeyboardButton("Отчет за сутки"),
                //                //new KeyboardButton("\"Call me ☎️"),
                //            },
                //            new[]
                //            {
                //                new KeyboardButton("Геолокация"),
                //                new KeyboardButton("Оборудование"),
                //            },
                //            new[]
                //            {
                //                new KeyboardButton("Геолокация"),
                //                new KeyboardButton("Настройки"),
                //            }
                //        })
                //        {
                //            ResizeKeyboard = true
                //        },
                //        parseMode: ParseMode.Html);

                //}
                //catch (Exception e)
                //{

                //    Console.WriteLine(e);
                //}



                foreach (var r1 in res.Groups)
                {
                    var sb = new StringBuilder();
                    sb.Append($"<b>#⃣ {r1.Name} #⃣</b>\n");
                    //sb.Append($"<a href='https://kmary.ru'>📆</a> {res.Dt_start.ToString("g")} - {res.Dt_end.ToString("g")} \n\n");
                    //var t1 = r1.Users.Select(e => e.Total +" "+e.Name).ToArray();
                    var empty = r1.Users.Where(e => e.Details.Count > 0);
                    //sb.AppendJoin("\n", r1.Users.Select(e => e.Name = $"🙎‍♂️ {e.Name} 👉 {e.Total + " " + e.Details.LastOrDefault(e => e.Dt_out > DateTime.Parse("2024-01-16 9:00:00.00"))?.Time}"));
                    //sb.AppendJoin("\n", empty.Select(e => e.Name = $"🙎‍♂️ {e.Name} 👉 {e.Total + " " + e.Details?.LastOrDefault(e => e.Dt_out > DateTime.Parse("2024-01-24 9:00:00.00"))?.Time}"));
                     sb.AppendJoin("\n", empty.Select(e => e.Name = $"🙎‍♂️ {e.Name} 👉 {e.Total}"));
                    //sb.Append(string.Join("\n",r1.Users.Select(e => e.Name).ToList()));
                    try
                    {
                        ////
                        //await client.EditMessageTextAsync("6310947780", 222435, sb.ToString(), ParseMode.Html);
                        ////var t = await client.EditMessageTextAsync(6310947780, 222351, "100000", parseMode: ParseMode.Html);
                        //Console.WriteLine();

                        var m = await client.SendTextMessageAsync(message.Chat.Id, sb.ToString(),
                            replyMarkup: new ReplyKeyboardMarkup(new[]
                            {
                            new[]
                            {
                                new KeyboardButton("⚙ Кто на работе"),
                                new KeyboardButton("Отчет за сутки"),
                                //new KeyboardButton("\"Call me ☎️"),
                            },
                            new[]
                            {
                                new KeyboardButton("Геолокация"),
                                new KeyboardButton("Мои отчеты"),
                            },
                            new[]
                            {
                                new KeyboardButton("Геолокация"),
                                new KeyboardButton("Настройки"),
                            }
                            })
                            {
                                ResizeKeyboard = true
                            },
                            parseMode: ParseMode.Html);
                        //var iniButtons = new List<InlineKeyboardButton>()
                        //{
                        //    InlineKeyboardButton.WithCallbackData($"Подробнее", "select_date")
                        //};

                        //InlineKeyboardMarkup detainsKeyboardMarkup = new(new[]
                        //{
                        //    iniButtons
                        //});

                        //var m = await client.SendTextMessageAsync(message.Chat.Id, sb.ToString(),
                        //    replyMarkup: detainsKeyboardMarkup, parseMode: ParseMode.Html);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }


                }

                break;
        }
        case "Настройки":
        {
            
            InlineKeyboardMarkup InlineKeyboard = new(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData($"{DateTime.Now.AddDays(-1)}", "select_date_start"),
                    InlineKeyboardButton.WithCallbackData($"<<", "select_time_start_1"),
                    InlineKeyboardButton.WithCallbackData($"06:00", "select_time_start"),
                    InlineKeyboardButton.WithCallbackData($">>", "select_time_start_1"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData($"{DateTime.Now:d}", "select_date_start"),
                    InlineKeyboardButton.WithCallbackData($"09:00", "select_time_start")
                }
            });

            await client.SendTextMessageAsync(message.Chat.Id, "1", replyMarkup: InlineKeyboard,
                parseMode: ParseMode.Html);
            
            //await client.DeleteMessageAsync(message.Chat.Id, message.MessageId, CancellationToken.None);
            break;
        }
    }


    //return Task.CompletedTask;
}



async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
{

    await (update.Type switch
    {
        UpdateType.Message => BotMessageReceived(client, update.Message),
        UpdateType.Unknown => throw new NotImplementedException(),
        UpdateType.CallbackQuery => HandleCallbackQuery(client, update.CallbackQuery!),
        UpdateType.EditedChannelPost => throw new NotImplementedException(),
        UpdateType.ShippingQuery => throw new NotImplementedException(),
        UpdateType.PreCheckoutQuery => throw new NotImplementedException(),
        UpdateType.Poll => throw new NotImplementedException(),
        UpdateType.PollAnswer => throw new NotImplementedException(),
        UpdateType.MyChatMember => throw new NotImplementedException(),
        UpdateType.ChatMember => throw new NotImplementedException(),
        UpdateType.ChatJoinRequest => throw new NotImplementedException(),
        _ => throw new NotImplementedException(),
    });




    //var GroupsKeyboard = new ReplyKeyboardMarkup(new[]
    //        {
    //        new[]
    //        {
    //            new KeyboardButton("Производство упаковочных материалов"),
    //            new KeyboardButton("Участок резки, упаковки и прессования ОДИ"),
    //            //new KeyboardButton("\"Call me ☎️"),
    //        },
    //        new[]
    //        {
    //            new KeyboardButton("Участок печати"),
    //            new KeyboardButton("Техническая служба"),
    //        },
    //        new[]
    //        {
    //            new KeyboardButton("Участок составления красок"),
    //            new KeyboardButton("Участок ламинации и пакетоделательных машин"),
    //        },
    //        new[]
    //        {
    //            new KeyboardButton("Служба качества"),
    //            new KeyboardButton("Дизайн студия"),
    //        }
    //    })
    //{ ResizeKeyboard = true };





    //if (update.Type == UpdateType.CallbackQuery)
    //{
    //    await HandleCallbackQuery(client, update.CallbackQuery);
    //    Console.WriteLine(update.CallbackQuery.Data);
    //    await client.SendTextMessageAsync(6310947780, "что еще", replyMarkup: keyboard);
    //}








    //var message = update.Message;
    ////var message = update.ChannelPost;
    //if (message?.Text == "/start")
    //{
    //    var cl = new Client("http://localhost:5222", new HttpClient());

    //    var res = await cl.ReportAsync(DateTimeOffset.Parse("2024-01-01 17:14:37.00"), DateTimeOffset.Now, new List<int> { 141 }, new List<int>() { 142 });
        
    //    //InlineKeyboardMarkup InlineKeyboard1 = new(new[]
    //    //{
    //    //    new[]
    //    //    {
    //    //        InlineKeyboardButton.WithCallbackData("Buy 50c1", "buy_50c1"),
    //    //        InlineKeyboardButton.WithCallbackData("Buy 100c", "buy_100c"),
    //    //    },
    //    //    new[]
    //    //    {
    //    //        InlineKeyboardButton.WithCallbackData("Sell 50c", "sell_50c"),
    //    //        InlineKeyboardButton.WithCallbackData("Sell 100c", "sell_100c"),
    //    //    },
    //    //});

    //    var r = string.Join("\n", res.Groups.Select(e => e.Name).ToList());

    //    foreach (var r1 in res.Groups)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append($"<b>\U0001F527{r1.Name} {char.ConvertFromUtf32(0x2705)}</b>\n");
    //        //var t1 = r1.Users.Select(e => e.Total +" "+e.Name).ToArray();
    //        sb.AppendJoin("\n", r1.Users.Select(e => e.Name = $"{"\U00000031"}{"\U000020E3"}{e.Name} {e.Total}"));
    //        //sb.Append("\n\n");
    //        //sb.Append(string.Join("\n",r1.Users.Select(e => e.Name).ToList()));
      //      await client.SendTextMessageAsync(message.Chat.Id, sb.ToString(), replyMarkup: keyboard, parseMode: ParseMode.Html, cancellationToken:token);
    //    }


    //}


    async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {

        InlineKeyboardMarkup InlineKeyboard1 = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(DateTime.Now.ToString("d"), "select_date"),
                //InlineKeyboardButton.WithCallbackData(DateTime.Now.ToString("d"), "select_date1")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(DateTime.Now.AddDays(-1).ToString("d"), "date_start"),
                InlineKeyboardButton.WithCallbackData(DateTime.Now.AddDays(1).ToString("d"), "date_end"),
            }
        });



        if (callbackQuery.Data.StartsWith("select_date"))
        {


            try
            {
                await botClient.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, messageId: callbackQuery.Message.MessageId, replyMarkup: InlineKeyboard1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            //try
            //{
            //    await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, messageId: callbackQuery.Message.MessageId,
            //        msg, replyMarkup: InlineKeyboard1, parseMode:ParseMode.Html);
            //    return;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
            
        }

    }



}

async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
{
    //throw new NotImplementedException();
}
