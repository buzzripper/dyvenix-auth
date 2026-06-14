using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Dyvenix.Auth.Api.Endpoints.v1;

public static class RoleEndpoints
{
	public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/v1/role")
			.WithTags("Role");

		group.MapGet("GetById/{id}", GetById)
			.Produces<RoleDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapGet("GetByName/{name}", GetByName)
			.Produces<RoleDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapGet("GetAllByTenant/{tenantId}", GetAllByTenant)
			.Produces<IReadOnlyList<RoleDto>>(StatusCodes.Status200OK);

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

	public static async Task<Result<RoleDto?>> GetById(IRoleService roleService, string id)
	{
		var dto = await roleService.GetById(id);
		return Result<RoleDto?>.Ok(dto);
	}

	public static async Task<Result<RoleDto?>> GetByName(IRoleService roleService, string name)
	{
		var dto = await roleService.GetByName(Uri.UnescapeDataString(name));
		return Result<RoleDto?>.Ok(dto);
	}

	public static async Task<Result<IReadOnlyList<RoleDto>>> GetAllByTenant(IRoleService roleService, Guid tenantId)
	{
		var data = await roleService.GetAllByTenant(tenantId);
		return Result<IReadOnlyList<RoleDto>>.Ok(data);
	}

	public static async Task<Result<string>> Create(IRoleService roleService, [FromBody] CreateRoleReq request)
	{
		var id = await roleService.Create(request);
		return Result<string>.Ok(id);
	}

	public static async Task<Result> Update(IRoleService roleService, [FromBody] UpdateRoleReq request)
	{
		await roleService.Update(request);
		return Result.Ok();
	}

	public static async Task<Result> Delete(IRoleService roleService, string id)
	{
		await roleService.Delete(id);
		return Result.Ok();
	}
}
