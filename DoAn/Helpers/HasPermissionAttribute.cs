using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class HasPermissionAttribute : ActionFilterAttribute
{
    private readonly string _permission;

    public HasPermissionAttribute(string permission)
    {
        _permission = permission;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", new { area = "Admin" });
            return;
        }

        var permissionClaim = user.Claims.FirstOrDefault(c => c.Type == "permissions")?.Value;

        if (permissionClaim == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var permissions = permissionClaim.Split(",");

        if (!permissions.Contains(_permission))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Error", new { area = "" });
            return;
        }
    }
}
