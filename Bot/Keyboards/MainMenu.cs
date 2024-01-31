using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Keyboards
{
    internal class MainMenu
    {


        InlineKeyboardMarkup InlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData($"{DateTime.Now.ToString("d")}" +
                                                      $"669869698698698", "select_date")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData($"{DateTime.Now.AddDays(-1):d}\n\r 06:00", "date_start"),
                InlineKeyboardButton.WithCallbackData($"{DateTime.Now.AddDays(1):d}\n\r 09:00", "date_end"),
            }
        });
    }
}
