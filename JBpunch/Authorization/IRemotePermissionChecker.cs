namespace JBpunch.Authorization;

public interface IRemotePermissionChecker
{
    Task<bool> IsGrantedAsync(string permissionName);
}
