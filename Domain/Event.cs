using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain;

[Index(nameof(Code), nameof(ReaderId), nameof(UserId), nameof(DateTime), IsUnique = true)]
public class Event
{
    public int Id { get; set; }

    //[Column("code")]
    public short Code { get; set; }
    public string Message => Code switch
    {
        1 => "Нет доступа. Неразрешенный ключ",
        2 => "Проход по ключу разрешен",
        8 => "Проход совершен",
        _ => throw new ArgumentOutOfRangeException()
    };

    //[Column("reader_id")]
    public short ReaderId { get; set; }
    public Reader Reader { get; set; } = new();

    //[Column("user_id")]
    public int UserId { get; set; }
    public User? User { get; set; }

    [Column("dateTime")]
    [Precision(2)]
    public DateTime DateTime { get; set; }
}