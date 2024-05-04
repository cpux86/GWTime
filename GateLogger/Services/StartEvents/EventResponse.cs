using System.Text.Json.Serialization;

namespace GateLogger.Services.StartEvents;

public class EventResponse
{
    [JsonPropertyName("eventCode")] public short Code { get; set; }
    [JsonPropertyName("readerId")] public short ReaderId { get; set; }
    [JsonPropertyName("userId")] public int UserId { get; set; }
    [JsonPropertyName("dateTime")] public string DateTime { get; set; } = string.Empty;

    public bool alarm { get; set; }

    [JsonPropertyName("unit")] public string ReaderName { get; set; } = string.Empty;
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;


    #region User
    [JsonPropertyName("name")] public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("fio")] public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("group")] public string Group { get; set; } = string.Empty;

    #endregion

}