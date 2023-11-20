using System.ComponentModel.DataAnnotations.Schema;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    [Index(nameof(EventCode), nameof(ReaderId), nameof(UserId), nameof(DateTime), IsUnique = true)]
    public class Event {
        public int Id { get; set; }

        //[Column("eventCode")] 
        public byte EventCode { get; set; }

        //[Column("readerId")]
        public short ReaderId { get; set; }
        public Reader Reader { get; set; }


        //[Column("userId")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column("dateTime")]
        [Precision(2)]
        public DateTime DateTime { get; set; }
    }
}
