using DoAn.Models.Booking;
using System;
using System.Collections.Generic;

namespace DoAn.Models.Accounts;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; }

    public DateTime Birthday { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string Phone { get; set; }

    public int? RoleId { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool? IsActive { get; set; }
    public UserRole Role { get; set; }
    public virtual Membership Membership { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
