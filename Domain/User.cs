using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class User : BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    /// <summary>
    /// Фамилия и инициалы. Иванов И.И.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Ф.И.О полностью.  Иванов Иван Иванович
    /// </summary>
    public string FullName { get; set; } = string.Empty;
    /// <summary>
    /// События использования ключа
    /// </summary>
    public List<Event>? Events { get; set; } = new List<Event>();

    /// <summary>
    /// Группа в которую входит сотрудник
    /// </summary>
    public Group? Group { get; set; }

    /// <summary>
    /// RFID ключ
    /// </summary>
    public string? Key { get; set; } = string.Empty;

    /// <summary>
    /// Дата последнего использования ключа
    /// </summary>
    [Precision(2)]
    public DateTime? LastUsedKeyDate { get; set; } = DateTime.MinValue;
    /// <summary>
    /// Место последнего прохода
    /// </summary>
    public string? LastUsedReaderName { get; set; }

    public string? LastEventMessage { get; set; }
}