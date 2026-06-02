using Dyvenix.Auth.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dyvenix.Auth.Api.Endpoints;

public static class BrandImgEndpoints
{
	public static IEndpointRouteBuilder MapBrandImgEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/auth/brand-img")
			.WithTags("BrandImg");

		group.MapGet("{tenantSlug}", GetImage)
			.Produces(StatusCodes.Status200OK, contentType: "image/png")
			.Produces(StatusCodes.Status404NotFound)
			.AllowAnonymous();

		group.MapPost("{tenantSlug}", UploadImage)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status400BadRequest)
			.AllowAnonymous()
			.DisableAntiforgery();

		group.MapDelete("{tenantSlug}", DeleteImage)
			.Produces(StatusCodes.Status204NoContent)
			.AllowAnonymous();

		return app;
	}

	private static async Task<IResult> GetImage(string tenantSlug, BrandImgService service, CancellationToken ct)
	{
		var stream = await service.GetAsync(tenantSlug, ct);

		if (stream is null)
			return Results.NotFound();

		return Results.Stream(stream, "image/png", $"{tenantSlug}.png");
	}

	private static async Task<IResult> UploadImage(string tenantSlug, IFormFile file, BrandImgService service, CancellationToken ct)
	{
		if (file is null || file.Length == 0)
			return Results.BadRequest("No file provided.");

		await using var stream = file.OpenReadStream();
		await service.SaveAsync(tenantSlug, stream, ct);

		return Results.NoContent();
	}

	private static async Task<IResult> DeleteImage(string tenantSlug, BrandImgService service, CancellationToken ct)
	{
		await service.DeleteAsync(tenantSlug, ct);
		return Results.NoContent();
	}
}
