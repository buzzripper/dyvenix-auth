using Dyvenix.Auth.Shared.Authorization;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Dyvenix.Auth.Api.Endpoints.v1;

public static class UserEndpoints
{
	public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/v1/user")
			.WithTags("User");

		group.MapGet("GetById/{id}", GetById)
			.Produces<UserDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound)
			.RequireAuthorization(AuthPermissions.Read);

		group.MapGet("GetByEmail/{email}", GetByEmail)
			.Produces<UserDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound)
			.RequireAuthorization(AuthPermissions.Read);

		group.MapGet("GetAllByTenant/{tenantId}", GetAllByTenant)
			.Produces<IReadOnlyList<UserSummaryDto>>(StatusCodes.Status200OK);

		group.MapPost("Create", Create)
			.Produces<string>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.RequireAuthorization(AuthPermissions.Write);

		group.MapPut("Update", Update)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound)
			.RequireAuthorization(AuthPermissions.Write);

		group.MapDelete("Delete/{id}", Delete)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound)
			.RequireAuthorization(AuthPermissions.Write);

		return app;
	}

	public static async Task<Result<UserDto?>> GetById(string id, IUserService userService)
	{
		var dto = await userService.GetById(id);
		return Result<UserDto?>.Ok(dto);
	}

	public static async Task<Result<UserDto?>> GetByEmail(string email, IUserService userService)
	{
		var dto = await userService.GetByEmail(Uri.UnescapeDataString(email));
		return Result<UserDto?>.Ok(dto);
	}

	public static async Task<Result<IReadOnlyList<UserSummaryDto>>> GetAllByTenant(Guid tenantId, IUserService userService)
	{
		var data = await userService.GetAllByTenant(tenantId);
		return Result<IReadOnlyList<UserSummaryDto>>.Ok(data);
	}

	public static async Task<Result<string>> Create([FromBody] CreateUserReq request, IUserService userService)
	{
		var id = await userService.Create(request);
		return Result<string>.Ok(id);
	}

	public static async Task<Result> Update([FromBody] UpdateUserReq request, IUserService userService)
	{
		await userService.Update(request);
		return Result.Ok();
	}

	public static async Task<Result> Delete(string id, IUserService userService)
	{
		await userService.Delete(id);
		return Result.Ok();
	}
}
