using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Common.Shared.DTOs;

namespace Dyvenix.Auth.Endpoints.v1;

public static class UserClaimEndpoints
{
    public static IEndpointRouteBuilder MapUserClaimEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/userclaim")
            .WithTags("UserClaim");

        group.MapGet("GetById/{id}", GetById)
            .Produces<UserClaimDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("GetAllByUser/{userId}", GetAllByUser)
            .Produces<IReadOnlyList<UserClaimDto>>(StatusCodes.Status200OK);

        group.MapPost("Create", Create)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("Update", Update)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("Delete/{id}", Delete)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    public static async Task<Result<UserClaimDto?>> GetById(IUserClaimService userClaimService, int id)
    {
        var dto = await userClaimService.GetById(id);
        return Result<UserClaimDto?>.Ok(dto);
    }

    public static async Task<Result<IReadOnlyList<UserClaimDto>>> GetAllByUser(IUserClaimService userClaimService, string userId)
    {
        var data = await userClaimService.GetAllByUser(userId);
        return Result<IReadOnlyList<UserClaimDto>>.Ok(data);
    }

    public static async Task<Result> Create(IUserClaimService userClaimService, [FromBody] CreateUserClaimReq request)
    {
        await userClaimService.Create(request);
        return Result.Ok();
    }

    public static async Task<Result> Update(IUserClaimService userClaimService, [FromBody] UpdateUserClaimReq request)
    {
        await userClaimService.Update(request);
        return Result.Ok();
    }

    public static async Task<Result> Delete(IUserClaimService userClaimService, int id)
    {
        await userClaimService.Delete(id);
        return Result.Ok();
    }
}
