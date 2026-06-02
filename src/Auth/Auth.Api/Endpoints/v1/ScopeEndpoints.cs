using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Common.Shared.DTOs;

namespace Dyvenix.Auth.Endpoints.v1;

public static class ScopeEndpoints
{
    public static IEndpointRouteBuilder MapScopeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/scope")
            .WithTags("Scope");

        group.MapGet("GetById/{id}", GetById)
            .Produces<ScopeDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("GetByName/{name}", GetByName)
            .Produces<ScopeDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("GetAll", GetAll)
            .Produces<IReadOnlyList<ScopeDto>>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id}", Delete)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    public static async Task<Result<ScopeDto?>> GetById(IScopeService scopeService, string id)
    {
        var dto = await scopeService.GetById(id);
        return Result<ScopeDto?>.Ok(dto);
    }

    public static async Task<Result<ScopeDto?>> GetByName(IScopeService scopeService, string name)
    {
        var dto = await scopeService.GetByName(Uri.UnescapeDataString(name));
        return Result<ScopeDto?>.Ok(dto);
    }

    public static async Task<Result<IReadOnlyList<ScopeDto>>> GetAll(IScopeService scopeService)
    {
        var data = await scopeService.GetAll();
        return Result<IReadOnlyList<ScopeDto>>.Ok(data);
    }

    public static async Task<Result<string>> Create(IScopeService scopeService, [FromBody] CreateScopeReq request)
    {
        var id = await scopeService.Create(request);
        return Result<string>.Ok(id);
    }

    public static async Task<Result> Update(IScopeService scopeService, [FromBody] UpdateScopeReq request)
    {
        await scopeService.Update(request);
        return Result.Ok();
    }

    public static async Task<Result> Delete(IScopeService scopeService, string id)
    {
        await scopeService.Delete(id);
        return Result.Ok();
    }
}
