using System.Text.Json.Serialization;

namespace JBpunch.DTOs;

// Nullable primitives for OpenAPI schema generation
[JsonSerializable(typeof(int?))]
[JsonSerializable(typeof(Guid?))]
[JsonSerializable(typeof(DateTime?))]
// REST API DTOs
[JsonSerializable(typeof(ClockDataQueryDto))]
[JsonSerializable(typeof(CreateClockDataDto))]
[JsonSerializable(typeof(ClockDataResponseDto))]
[JsonSerializable(typeof(List<ClockDataResponseDto>))]
[JsonSerializable(typeof(CreateGpsPuncheDto))]
[JsonSerializable(typeof(UpdateGpsPuncheDto))]
[JsonSerializable(typeof(GpsPuncheResponseDto))]
[JsonSerializable(typeof(List<GpsPuncheResponseDto>))]
// System endpoints
[JsonSerializable(typeof(ServiceInfoDto))]
[JsonSerializable(typeof(AuthTestDto))]
[JsonSerializable(typeof(ClaimDto))]
[JsonSerializable(typeof(ClaimDto[]))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class AppJsonSerializerContext : JsonSerializerContext { }

// Named types for system endpoints (AOT requires no anonymous types)
public record ServiceInfoDto(
    string Service,
    string? PathBase,
    string Openapi,
    string Scalar,
    string Graphql,
    bool DBConnection
);

public record AuthTestDto(
    bool IsAuthenticated,
    string? AuthenticationType,
    string? UserName,
    ClaimDto[] Claims
);

public record ClaimDto(string Type, string Value);
