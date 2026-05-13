using Microsoft.AspNetCore.Authorization;

namespace JBpunch.Authorization;

public class AbpPermissionRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; }

    public AbpPermissionRequirement(string permissionName)
    {
        PermissionName = permissionName;
    }
}
