using Dyvenix.Auth.Shared.Contracts;
using Microsoft.Extensions.Logging;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats.Png;

namespace Dyvenix.Auth.Api.Services;

public class BrandImgService(IBrandImgRepository repository, ILogger<BrandImgService> logger)
{
	public async Task SaveAsync(string tenantSlug, Stream uploadStream, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tenantSlug);

		//using var image = await Image.LoadAsync(uploadStream, ct);

		//using var pngStream = new MemoryStream();
		//await image.SaveAsync(pngStream, new PngEncoder(), ct);
		//pngStream.Position = 0;

		await repository.SaveAsync(tenantSlug, uploadStream, ct);

		logger.LogInformation("Brand image saved for tenant {TenantSlug}", tenantSlug);
	}

	public async Task<Stream?> GetAsync(string tenantSlug, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tenantSlug);

		return await repository.GetAsync(tenantSlug, ct);
	}

	public async Task DeleteAsync(string tenantSlug, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tenantSlug);

		await repository.DeleteAsync(tenantSlug, ct);

		logger.LogInformation("Brand image deleted for tenant {TenantSlug}", tenantSlug);
	}
}
