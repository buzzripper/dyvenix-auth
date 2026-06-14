using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Dyvenix.Auth.Api.Endpoints.v1;

public static class OidcAppEndpoints
{
	public static IEndpointRouteBuilder MapOidcAppEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/v1/oidcapp")
			.WithTags("OidcApp");

		group.MapGet("GetById/{id}", GetById)
			.Produces<OidcAppDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapGet("GetByClientId/{clientId}", GetByClientId)
			.Produces<OidcAppDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapGet("GetAll", GetAll)
			.Produces<IReadOnlyList<OidcAppDto>>(StatusCodes.Status200OK);

		group.MapPost("Create", Create)
			.Produces<string>(StatusCodes.Status200OK);

		group.MapPut("Update", Update)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapDelete("Delete/{id}", Delete)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		return app;
	}

	public static async Task<Result<OidcAppDto?>> GetById(IOidcAppService oidcAppService, string id)
	{
		var dto = await oidcAppService.GetById(id);
		return Result<OidcAppDto?>.Ok(dto);
	}

	public static async Task<Result<OidcAppDto?>> GetByClientId(IOidcAppService oidcAppService, string clientId)
	{
		var dto = await oidcAppService.GetByClientId(Uri.UnescapeDataString(clientId));
		return Result<OidcAppDto?>.Ok(dto);
	}

	public static async Task<Result<IReadOnlyList<OidcAppDto>>> GetAll(IOidcAppService oidcAppService)
	{
		var data = await oidcAppService.GetAll();
		return Result<IReadOnlyList<OidcAppDto>>.Ok(data);
	}

	public static async Task<Result<string>> Create(IOidcAppService oidcAppService, [FromBody] CreateOidcAppReq request)
	{
		var id = await oidcAppService.Create(request);
		return Result<string>.Ok(id);
	}

	public static async Task<Result> Update(IOidcAppService oidcAppService, [FromBody] UpdateOidcAppReq request)
	{
		await oidcAppService.Update(request);
		return Result.Ok();
	}

	public static async Task<Result> Delete(IOidcAppService oidcAppService, string id)
	{
		await oidcAppService.Delete(id);
		return Result.Ok();
	}
}
