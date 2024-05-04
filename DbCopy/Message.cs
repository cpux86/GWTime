using System;
using System.Collections.Generic;

namespace DbCopy;

public partial class Message
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
