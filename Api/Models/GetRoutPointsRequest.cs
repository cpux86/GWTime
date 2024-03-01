namespace Api.Models
{
    public class WorkingDays
    {
        public int UserId { get; set; }
        public List<DateTime> DateList { get; set; }
    }
}