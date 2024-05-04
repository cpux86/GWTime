using System;
using System.Collections.Generic;

namespace DbCopy;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public int? UserGroupId { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual UserGroup? UserGroup { get; set; }
}
