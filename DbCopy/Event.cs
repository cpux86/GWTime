using System;
using System.Collections.Generic;

namespace dbCopy;

public partial class Event
{
    public int Id { get; set; }

    public short Code { get; set; }

    public short ReaderId { get; set; }

    public int UserId { get; set; }

    public DateTime DateTime { get; set; }

    public virtual Reader Reader { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
