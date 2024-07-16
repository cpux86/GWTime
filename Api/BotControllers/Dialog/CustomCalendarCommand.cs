using Newtonsoft.Json;
using PRTelegramBot.Models.CallbackCommands;

namespace Api.BotControllers.Dialog;

public class CustomCalendarCommand : CalendarTCommand
{
    [JsonProperty("uid")]
    public int UserId { get; set; }
    public CustomCalendarCommand(DateTime date, int userId, int command = 0) : base(date, command)
    {
        UserId = userId;
    }
}