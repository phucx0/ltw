namespace DoAn.Models.Accounts
{
    public class MembershipTier
    {
        public int TierId { get; set; }
        public string TierName { get; set; }
        public int MinPoints { get; set; }
        public int MaxPoints { get; set; }

        // Navigation properties
        public ICollection<Membership> Memberships { get; set; }
    }

}
