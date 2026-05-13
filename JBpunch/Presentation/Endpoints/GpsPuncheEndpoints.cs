using JBpunch.Authorization;
using JBpunch.DTOs;
using JBpunch.Services;

namespace JBpunch.Presentation.Endpoints;

public static class GpsPuncheEndpoints
{
    public static RouteGroupBuilder MapGpsPuncheEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/GpsPunche").WithTags("GpsPunche");

        group
            .MapGet(
                "/",
                async (IGpsPuncheService service) =>
                {
                    var result = await service.GetAllAsync();
                    return Results.Ok(result);
                }
            )
            .RequireAuthorization(JBpunchPermissions.GpsPuncheManage);

        group
            .MapGet(
                "/{id:guid}",
                async (Guid id, IGpsPuncheService service) =>
                {
                    var result = await service.GetByIdAsync(id);
                    return result is null ? Results.NotFound() : Results.Ok(result);
                }
            )
            .RequireAuthorization(JBpunchPermissions.GpsPuncheManage);

        group
            .MapPost(
                "/",
                async (CreateGpsPuncheDto dto, IGpsPuncheService service) =>
                {
                    var result = await service.CreateAsync(dto);
                    return Results.Created($"/api/GpsPunche/{result.Id}", result);
                }
            )
            .RequireAuthorization(JBpunchPermissions.GpsPuncheCreate);

        group
            .MapPut(
                "/{id:guid}",
                async (Guid id, UpdateGpsPuncheDto dto, IGpsPuncheService service) =>
                {
                    try
                    {
                        var result = await service.UpdateAsync(id, dto);
                        return Results.Ok(result);
                    }
                    catch (System.Collections.Generic.KeyNotFoundException)
                    {
                        return Results.NotFound();
                    }
                }
            )
            .RequireAuthorization(JBpunchPermissions.GpsPuncheCreate);

        group
            .MapDelete(
                "/{id:guid}",
                async (Guid id, IGpsPuncheService service) =>
                {
                    await service.DeleteAsync(id);
                    return Results.NoContent();
                }
            )
            .RequireAuthorization(JBpunchPermissions.GpsPuncheDelete);

        return group;
    }
}
