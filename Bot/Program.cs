using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


var client = new TelegramBotClient("6320526153:AAFLv4Y7DT1XqE6prz7Fjmt0fgW-5-yBGFo");
client.StartReceiving(Update, Error);
Console.ReadLine();
async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
{
    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Help me" },
        new KeyboardButton[] { "Call me ☎️" },
    })
    {
        ResizeKeyboard = true
    };

    var keyboard = new ReplyKeyboardMarkup(new[]
    {
        new[]
        {
            new KeyboardButton("Кто на работе"),
            new KeyboardButton("Отчет за сутки"),
            //new KeyboardButton("\"Call me ☎️"),
        },
        new[]
        {
            new KeyboardButton("Геолокация"),
            new KeyboardButton("Оборудование"),
        },
        new[]
        {
        new KeyboardButton("Хостел"),
        new KeyboardButton("Контакты"),
        },
        new[]
        {
            new KeyboardButton("Геолокация"),
            new KeyboardButton("Оборудование"),
        }
    })
    { ResizeKeyboard = true };

    //InlineKeyboardMarkup keyboard = new(new[]
    //    {
    //        new[]
    //        {
    //            InlineKeyboardButton.WithCallbackData("Buy 50c", "buy_50c"),
    //            InlineKeyboardButton.WithCallbackData("Buy 100c", "buy_100c"),
    //        },
    //        new[]
    //        {
    //            InlineKeyboardButton.WithCallbackData("Sell 50c", "sell_50c"),
    //            InlineKeyboardButton.WithCallbackData("Sell 100c", "sell_100c"),
    //        },
    //    });

    //if (update.Type == UpdateType.CallbackQuery)
    //{
    //    Console.WriteLine(update.CallbackQuery.Data);
    //    await client.SendTextMessageAsync(6310947780, "что еще", replyMarkup: keyboard);
    //}

    var message = update.Message;
    if(message.Text != null)
    {
        await client.SendTextMessageAsync(message.Chat.Id, message.Text, replyMarkup: keyboard);
    }

    //if (message.Text == "/inline")
    //{
    //    InlineKeyboardMarkup keyboard = new(new[]
    //    {
    //        new[]
    //        {
    //            InlineKeyboardButton.WithCallbackData("Buy 50c", "buy_50c"),
    //            InlineKeyboardButton.WithCallbackData("Buy 100c", "buy_100c"),
    //        },
    //        new[]
    //        {
    //            InlineKeyboardButton.WithCallbackData("Sell 50c", "sell_50c"),
    //            InlineKeyboardButton.WithCallbackData("Sell 100c", "sell_100c"),
    //        },
    //    });
    //    await client.SendTextMessageAsync(message.Chat.Id, "Choose inline:", replyMarkup: keyboard);
    //    return;
    //}

    async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        if (callbackQuery.Data.StartsWith("buy"))
        {
            await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"Вы хотите купить?"
            );
            return;
        }
        if (callbackQuery.Data.StartsWith("sell"))
        {
            await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"Вы хотите продать?"
            );
            return;
        }
        await botClient.SendTextMessageAsync(
            callbackQuery.Message.Chat.Id,
            $"You choose with data: {callbackQuery.Data}"
        );
        return;
    }



    //if (message.Text != null)
    //{
    //    Console.WriteLine(message.Text);
    //    //client.SendTextMessageAsync(chatId: 6310947780, text: $"Вы написали: {message.Text}", cancellationToken: CancellationToken.None);
    //    //client.SendTextMessageAsync(chatId: message.Chat.Id, text: $"{message.Text}", replyMarkup: keyboard, cancellationToken: CancellationToken.None);
    //    //await client.SendAnimationAsync(
    //    //    chatId: 6310947780,
    //    //    animation: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-waves.mp4"),
    //    //    caption: "Waves",
    //    //    cancellationToken: token);

    //    await client.SendTextMessageAsync(
    //        chatId: 6310947780,
    //        text: "Trying *all the parameters* of `sendMessage` method",
    //        parseMode: ParseMode.MarkdownV2,
    //        disableNotification: true,
    //        replyToMessageId: update.Message.MessageId,
    //        replyMarkup: new InlineKeyboardMarkup(
    //            InlineKeyboardButton.WithUrl(
    //                text: "Check sendMessage method",
    //                url: "https://core.telegram.org/bots/api#sendmessage")),
    //        cancellationToken: token);
    //}


}




async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
{
    //throw new NotImplementedException();
}
