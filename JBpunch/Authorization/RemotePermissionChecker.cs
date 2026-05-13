using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace JBpunch.Authorization;

/// <summary>
/// 透過呼叫 AuthServer 的 /api/abp/application-configuration 檢查權限。
/// 此端點任何已認證使用者皆可存取，不需要 admin 權限。
/// </summary>
public class RemotePermissionChecker : IRemotePermissionChecker
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RemotePermissionChecker> _logger;

    public RemotePermissionChecker(
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<RemotePermissionChecker> logger
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> IsGrantedAsync(string permissionName)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var userId =
            httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContext.User.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("無法取得使用者 ID");
            return false;
        }

        var grantedPolicies = await FetchGrantedPoliciesAsync(httpContext);

        var result = grantedPolicies?.Contains(permissionName) ?? false;
        _logger.LogDebug(
            "權限檢查: {Permission} = {Result} (使用者: {UserId})",
            permissionName,
            result,
            userId
        );
        return result;
    }

    /// <summary>
    /// 從 ABP AuthServer 的 /api/abp/application-configuration 取得當前使用者的已授予權限。
    /// 回應格式: { "auth": { "grantedPolicies": { "CheckIn.ClockData": true, ... } } }
    /// </summary>
    private async Task<HashSet<string>?> FetchGrantedPoliciesAsync(HttpContext httpContext)
    {
        var authority = _configuration["AuthServer:Authority"]?.TrimEnd('/');
        if (string.IsNullOrEmpty(authority))
        {
            _logger.LogError("AuthServer:Authority 未設定");
            return null;
        }

        var configUrl = $"{authority}/api/abp/application-configuration";

        var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
        if (
            string.IsNullOrEmpty(authHeader)
            || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
        )
        {
            _logger.LogWarning("無法取得 Bearer Token");
            return null;
        }

        var token = authHeader["Bearer ".Length..];

        try
        {
            var client = _httpClientFactory.CreateClient("AuthServer");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await client.GetAsync(configUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "ABP application-configuration API 回傳錯誤: {StatusCode}",
                    response.StatusCode
                );
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (
                !root.TryGetProperty("auth", out var authElement)
                || !authElement.TryGetProperty("grantedPolicies", out var policiesElement)
            )
            {
                _logger.LogWarning(
                    "ABP application-configuration 回應中找不到 auth.grantedPolicies"
                );
                return null;
            }

            var policies = new HashSet<string>(StringComparer.Ordinal);
            foreach (var prop in policiesElement.EnumerateObject())
            {
                if (prop.Value.GetBoolean())
                {
                    policies.Add(prop.Name);
                }
            }

            _logger.LogInformation("取得使用者已授予權限共 {Count} 筆", policies.Count);
            return policies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "呼叫 ABP application-configuration API 時發生錯誤");
            return null;
        }
    }
}
