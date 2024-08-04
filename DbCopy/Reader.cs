using System;
using System.Collections.Generic;

namespace dbCopy;

public partial class Reader
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
