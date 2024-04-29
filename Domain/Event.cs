using System.ComponentModel.DataAnnotations.Schema;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    [Index(nameof(MessageId), nameof(ReaderId), nameof(UserId), nameof(DateTime), IsUnique = true)]
    public class Event {
        public int Id { get; set; }

        //[Column("eventCode")] 
        public short MessageId { get; set; }
        public Message Message { get; set; } = new();

        //[Column("readerId")]
        public short ReaderId { get; set; }
        public Reader Reader { get; set; } = new();


        //[Column("userId")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column("dateTime")]
        [Precision(2)]
        public DateTime DateTime { get; set; }
    }
}
