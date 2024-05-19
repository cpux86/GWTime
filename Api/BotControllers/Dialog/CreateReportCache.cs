using PRTelegramBot.Interface;

namespace Api.BotControllers.Dialog
{
    public class CreateReportCache : ITelegramCache
    {
        public ReportType Type { get; set; } = ReportType.Quick;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Text { get; set; }
        public bool ClearData()
        {
            this.Text = string.Empty;
            return true;
            
        }
    }
}
