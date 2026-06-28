using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Dyvenix.Auth.Api.Endpoints.v1;

public static class TenantEndpoints
{
	public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/v1/tenant")
			.WithTags("Tenant");

		group.MapGet("GetById/{id}", GetById)
			.Produces<TenantDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapGet("GetByKey/{key}", GetByKey)
			.Produces<TenantDto>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapGet("GetAll", GetAll)
			.Produces<IReadOnlyList<TenantDto>>(StatusCodes.Status200OK);

		group.MapPost("Create", Create)
			.Produces<Guid>(StatusCodes.Status200OK);

		group.MapPut("Update", Update)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		group.MapDelete("Delete", Delete)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound);

		return app;
	}

	public static async Task<Result<TenantDto?>> GetById(ITenantService tenantService, Guid id)
	{
		var dto = await tenantService.GetById(id);
		return Result<TenantDto?>.Ok(dto);
	}

	public static async Task<Result<TenantDto?>> GetByKey(ITenantService tenantService, string key)
	{
		var dto = await tenantService.GetByKey(Uri.UnescapeDataString(key));
		return Result<TenantDto?>.Ok(dto);
	}

	public static async Task<Result<IReadOnlyList<TenantDto>>> GetAll(ITenantService tenantService)
	{
		var data = await tenantService.GetAll();
		return Result<IReadOnlyList<TenantDto>>.Ok(data);
	}

	public static async Task<Result<Guid>> Create(ITenantService tenantService, [FromBody] CreateTenantReq request)
	{
		var id = await tenantService.Create(request);
		return Result<Guid>.Ok(id);
	}

	public static async Task<Result> Update(ITenantService tenantService, [FromBody] UpdateTenantReq request)
	{
		await tenantService.Update(request);
		return Result.Ok();
	}

	public static async Task<Result> Delete(ITenantService tenantService, Guid id)
	{
		await tenantService.Delete(id);
		return Result.Ok();
	}
}
