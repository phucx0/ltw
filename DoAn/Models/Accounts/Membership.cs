using System;
using System.Collections.Generic;

namespace DoAn.Models.Accounts;

public partial class Membership
{
    public int MembershipId { get; set; }

    public int UserId { get; set; }

    public int? Points { get; set; }

    public string? Tier { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
