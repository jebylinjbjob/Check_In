using JBpunch.Authorization;
using JBpunch.DTOs;
using JBpunch.Services;

namespace JBpunch.Presentation.Endpoints;

public static class ClockDataEndpoints
{
    public static RouteGroupBuilder MapClockDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ClockData").WithTags("ClockData");

        group
            .MapGet(
                "/",
                async (
                    Guid? identityUserId,
                    DateTime? from,
                    DateTime? to,
                    int? limit,
                    IClockDataService service
                ) =>
                {
                    var query = new ClockDataQueryDto
                    {
                        IdentityUserId = identityUserId,
                        From = from,
                        To = to,
                        Limit = limit ?? 100,
                    };
                    var result = await service.GetAllAsync(query);
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(JBpunchPermissions.ClockDataManage);

        group
            .MapGet(
                "/{id:guid}",
                async (Guid id, IClockDataService service) =>
                {
                    var result = await service.GetByIdAsync(id);
                    return result is null ? Results.NotFound() : Results.Ok(result);
                }
            )
            .RequireAuthorization(JBpunchPermissions.ClockDataManage);

        group
            .MapPost(
                "/",
                async (CreateClockDataDto dto, IClockDataService service) =>
                {
                    var result = await service.CreateAsync(dto);
                    return Results.Created($"/api/ClockData/{result.Id}", result);
                }
            )
            .RequireAuthorization(JBpunchPermissions.ClockDataCreate);

        group
            .MapDelete(
                "/{id:guid}",
                async (Guid id, IClockDataService service) =>
                {
                    await service.DeleteAsync(id);
                    return Results.NoContent();
                }
            )
            .RequireAuthorization(JBpunchPermissions.ClockDataDelete);

        return group;
    }
}
