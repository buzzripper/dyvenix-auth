using Dyvenix.Auth.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;

namespace Dyvenix.Auth.Api.Endpoints;

public static class TenantAssetEndpoints
{
	private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

	public static IEndpointRouteBuilder MapTenantAssetEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/tenantassets")
			.WithTags("TenantAssets");

		group.MapGet("{tenantId:guid}/{**assetPath}", GetAsset)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		group.MapGet("bykey/{tenantKey}/{**assetPath}", GetAssetByTenantKey)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		group.MapPost("{tenantId:guid}/{**assetPath}", UploadAsset)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous()
			.DisableAntiforgery();

		group.MapDelete("{tenantId:guid}/{**assetPath}", DeleteAsset)
			.Produces(StatusCodes.Status204NoContent)
			.AllowAnonymous();

		return app;
	}

	private static async Task<IResult> GetAsset(Guid tenantId, string assetPath, ITenantAssetService service, CancellationToken ct)
	{
		var stream = await service.Get(tenantId, assetPath, ct);

		if (stream is null)
			return Results.NotFound();

		var contentType = GetContentType(assetPath);
		return Results.Stream(stream, contentType, Path.GetFileName(assetPath));
	}

	private static async Task<IResult> GetAssetByTenantKey(string tenantKey, string assetPath, ITenantAssetService service, CancellationToken ct)
	{
		var stream = await service.GetByTenantKey(tenantKey, assetPath, ct);

		if (stream is null)
			return Results.NotFound();

		var contentType = GetContentType(assetPath);
		return Results.Stream(stream, contentType, Path.GetFileName(assetPath));
	}

	private static async Task<IResult> UploadAsset(Guid tenantId, string assetPath, IFormFile file, ITenantAssetService service, CancellationToken ct)
	{
		if (file is null || file.Length == 0)
			return Results.BadRequest("No file provided.");

		await using var stream = file.OpenReadStream();
		await service.Save(tenantId, assetPath, stream, ct);

		return Results.NoContent();
	}

	private static async Task<IResult> DeleteAsset(Guid tenantId, string assetPath, ITenantAssetService service, CancellationToken ct)
	{
		await service.Delete(tenantId, assetPath, ct);
		return Results.NoContent();
	}

	private static string GetContentType(string path)
	{
		return _contentTypeProvider.TryGetContentType(path, out var contentType)
			? contentType
			: "application/octet-stream";
	}
}
