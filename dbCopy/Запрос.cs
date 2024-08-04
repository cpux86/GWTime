using System;
using System.Collections.Generic;

namespace dbCopy;

public partial class Запрос
{
    public short? Code { get; set; }

    public short ReaderId { get; set; }

    public int UserId { get; set; }

    public DateOnly DateTime { get; set; }
}
