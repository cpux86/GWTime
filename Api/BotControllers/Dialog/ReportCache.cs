using PRTelegramBot.Interface;

namespace Api.BotControllers.Dialog
{
    public class ReportCache : ITelegramCache
    {
        public string Text { get; set; }
        public bool ClearData()
        {
            this.Text = string.Empty;
            return true;
            
        }
    }
}
