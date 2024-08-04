using System;
using System.Collections.Generic;

namespace dbCopy;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public int? GroupId { get; set; }

    public string? Key { get; set; }

    public DateTime? LastUsedKeyDate { get; set; }

    public string? LastUsedReaderName { get; set; }

    public string? LastEventMessage { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual Group? Group { get; set; }
}
