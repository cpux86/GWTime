using System.Text.Json.Serialization;

namespace GateLogger.Services.StartEvents
{
    public class StartEventsResponse
    {
        [JsonPropertyName("command")]
        public string Command { get; set; } = string.Empty;

        [JsonPropertyName("event")]
        public EventResponse? Event { get; set; }
    }
}
