using DoAn.Models.Data;

public class PermissionService
{
    private readonly IDbContextFactory _dbFactory;
    public PermissionService(IDbContextFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public List<string> GetPermissionsByRoleId(int roleId)
    {
        var db = _dbFactory.Create("MOVIE_TICKET", "app_user", "app123");
        var permissions = (from rp in db.UserRolePermissions
                           join p in db.Permissions on rp.PermissionId equals p.PermissionId
                           where rp.RoleId == roleId
                           select p.PermissionName).ToList();

        return permissions;
    }
}
