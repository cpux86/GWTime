using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Domain;

public class Message
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public short Id { get; set; }
    public string Text { get; set; } = string.Empty;

    //public string TextTest => Id switch
    //{
    //    1 => "Нет доступа. Неразрешенный ключ",
    //    2 => "Проход по ключу разрешен",
    //    8 => "Проход совершен",
    //    _ => throw new ArgumentOutOfRangeException()
    //};

}