using Microsoft.AspNetCore.Authorization;

namespace JBpunch.Authorization;

public class AbpPermissionHandler : AuthorizationHandler<AbpPermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AbpPermissionHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AbpPermissionRequirement requirement
    )
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var permissionChecker =
            scope.ServiceProvider.GetRequiredService<IRemotePermissionChecker>();

        if (await permissionChecker.IsGrantedAsync(requirement.PermissionName))
        {
            context.Succeed(requirement);
        }
    }
}
