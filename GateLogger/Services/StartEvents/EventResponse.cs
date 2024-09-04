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
    public OtherInfo OtherInfo => new()
    {
        det1 = this.det1,
        det2 = this.det2,
        det3 = this.det3,
        det4 = this.det4,
        det5 = this.det5,
        det6 = this.det6,
        det7 = this.det7,
        det8 = this.det8,

    };

    #region OtherInfo

    public string det1 { get; set; }
    public string det2 { get; set; }
    public string det3 { get; set; }
    public string det4 { get; set; }
    public string det5 { get; set; }
    public string det6 { get; set; }
    public string det7 { get; set; }
    public string det8 { get; set; }

    #endregion

}

public class OtherInfo
{
    public string det1 { get; set; }
    public string det2 { get; set; }
    public string det3 { get; set; }
    public string det4 { get; set; }
    public string det5 { get; set; }
    public string det6 { get; set; }
    public string det7 { get; set; }
    public string det8 { get; set; }
}