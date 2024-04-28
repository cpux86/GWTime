using Domain;
using System.Text.Json.Serialization;

namespace GateLogger.Services.StartEvents
{
    public class EventResponse
    {
        [JsonPropertyName("eventCode")] public int EventCode { get; set; }
        [JsonPropertyName("readerId")] public int ReaderId { get; set; }
        [JsonPropertyName("userId")] public int UserId { get; set; }
        [JsonPropertyName("dateTime")] public string DateTime { get; set; } = string.Empty;

        //public EventType EventType => new()
        //{
        //    Id = (byte)EventCode,
        //    Name = Message
        //};

        public bool alarm { get; set; }

        [JsonPropertyName("unit")] public string ReaderName { get; set; } = string.Empty;
        [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;


        #region User
        [JsonPropertyName("name")] public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("fio")] public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("group")] public string Group { get; set; } = string.Empty;

        #endregion

    }

}
