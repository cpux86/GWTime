using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace dbCopy;

public partial class Event_sqlite
{
    public string? Index { get; set; }

    public DateTime? DateTime { get; set; }

    public string? EventType { get; set; }

    public string? EventCode { get; set; }

    public string? DevPtr { get; set; }

    public string? RdrPtr { get; set; }

    public string? UserPtr { get; set; }

    public string? OperatorId { get; set; }

    public string? AlarmStatus { get; set; }

    public string? Unit { get; set; }

    public string? Message { get; set; }

    public string? Name { get; set; }
}
