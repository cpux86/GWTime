using System.Text.Json.Serialization;

namespace GateLogger.Services.StartEvents
{
    public class StartEventsCommand
    {
        [JsonPropertyName("command")]
        public string Command { get; set; } = "startEvents";

        [JsonPropertyName("result")]
        public int Result { get; set; } = 1;
    }
}
