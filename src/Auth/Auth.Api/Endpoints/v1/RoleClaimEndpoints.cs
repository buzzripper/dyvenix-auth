using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Dyvenix.Auth.Api.Endpoints.v1;

public static class RoleClaimEndpoints
{
	public static IEndpointRouteBuilder MapRoleClaimEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/v1/roleclaim")
			.WithTags("RoleClaim");

		group.MapGet("GetById/{id}", GetById)
			.Produces<RoleClaimDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapGet("GetAllByRole/{roleId}", GetAllByRole)
			.Produces<IReadOnlyList<RoleClaimDto>>(StatusCodes.Status200OK);

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

	public static async Task<Result<RoleClaimDto?>> GetById(IRoleClaimService roleClaimService, int id)
	{
		var dto = await roleClaimService.GetById(id);
		return Result<RoleClaimDto?>.Ok(dto);
	}

	public static async Task<Result<IReadOnlyList<RoleClaimDto>>> GetAllByRole(IRoleClaimService roleClaimService, string roleId)
	{
		var data = await roleClaimService.GetAllByRole(roleId);
		return Result<IReadOnlyList<RoleClaimDto>>.Ok(data);
	}

	public static async Task<Result> Create(IRoleClaimService roleClaimService, [FromBody] CreateRoleClaimReq request)
	{
		await roleClaimService.Create(request);
		return Result.Ok();
	}

	public static async Task<Result> Update(IRoleClaimService roleClaimService, [FromBody] UpdateRoleClaimReq request)
	{
		await roleClaimService.Update(request);
		return Result.Ok();
	}

	public static async Task<Result> Delete(IRoleClaimService roleClaimService, int id)
	{
		await roleClaimService.Delete(id);
		return Result.Ok();
	}
}
