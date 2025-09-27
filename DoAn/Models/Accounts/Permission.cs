namespace DoAn.Models.Accounts
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescription { get; set; }
        public ICollection<UserRolePermission> RolePermissions { get; set; }
    }
}
