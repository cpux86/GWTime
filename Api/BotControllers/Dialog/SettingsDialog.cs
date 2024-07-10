using Api.BotControllers.Keyboard;
using PRTelegramBot.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Api.BotControllers.Dialog
{
    public class SettingsDialog
    {
        [ReplyMenuHandler("🛠 Настройка")]
        public async Task SettingReportCommand(ITelegramBotClient client, Update update)
        {

            var optionMessage = Menu.SettingsKeyboard();
            

            await PRTelegramBot.Helpers.Message.Send(client, update, "Настройки", optionMessage);
        }

        [ReplyMenuHandler("Мои люди")]
        public async Task MyPeopleCommand(ITelegramBotClient client, Update update)
        {

            var optionMessage = Menu.MyPeopleKeyboard();


            await PRTelegramBot.Helpers.Message.Send(client, update, "Настройки", optionMessage);
        }

    }
}
