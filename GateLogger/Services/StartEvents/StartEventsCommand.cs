using System.Text.Json.Serialization;

namespace GateLogger.Services.StartEvents
{
    public class StartEventsCommand
    {
        [JsonPropertyName("command")]
        public string Command { get; set; } = "startEvents";
        //public string command { get; set; } = "getConfig";

        [JsonPropertyName("result")]
        public int Result { get; set; } = 1;
    }
}
