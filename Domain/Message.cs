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

}