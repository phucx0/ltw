using System;
using System.Collections.Generic;

namespace DoAn.Models.Cinema;

public partial class Branch
{
    public int BranchId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
