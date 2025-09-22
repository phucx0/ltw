namespace DoAn.Models.Accounts
{
    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public ICollection<User> Users { get; set; }
    }

}
