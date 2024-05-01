using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain;

[Index(nameof(MessageId), nameof(ReaderId), nameof(UserId), nameof(DateTime), IsUnique = true)]
public class Event {
    public int Id { get; set; }

    //[Column("eventCode")] 
    public short MessageId { get; set; }

    public Message Message { get; set; } = new();

    public string GetMessage => MessageId switch
    {
        1 => "Нет доступа. Неразрешенный ключ",
        2 => "Проход по ключу разрешен",
        8 => "Проход совершен",
        _ => throw new ArgumentOutOfRangeException()
    };

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