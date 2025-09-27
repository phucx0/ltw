namespace DoAn.Models.Accounts
{
    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<UserRolePermission> RolePermissions { get; set; }
    }

}
