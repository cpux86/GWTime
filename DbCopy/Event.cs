using System;
using System.Collections.Generic;

namespace DbCopy;

public partial class Event
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public short ReaderId { get; set; }

    public int UserId { get; set; }

    public DateTime DateTime { get; set; }

    public virtual Message Message { get; set; } = null!;

    public virtual Reader Reader { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
