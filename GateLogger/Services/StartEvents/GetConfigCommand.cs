namespace GateLogger.Services.StartEvents
{
    public class GetConfigCommand
    {
        public string command { get; set; } = "getConfig";
        public int? result { get; set; } = 1;
    }
}
