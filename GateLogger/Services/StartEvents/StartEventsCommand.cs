namespace GateLogger.Services.StartEvents
{
    public class StartEventsCommand
    {
        public string command { get; set; } = "startEvents";
        //public string command { get; set; } = "getConfig";
        public int? result { get; set; } = 1;
    }
}
