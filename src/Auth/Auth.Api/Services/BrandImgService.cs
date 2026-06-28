using Dyvenix.Auth.Shared.Contracts;
using Microsoft.Extensions.Logging;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.Formats.Png;

namespace Dyvenix.Auth.Api.Services;

public class BrandImgService(IBrandImgRepository repository, ILogger<BrandImgService> logger)
{
	public async Task SaveAsync(string tenantKey, Stream uploadStream, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tenantKey);

		//using var image = await Image.LoadAsync(uploadStream, ct);

		//using var pngStream = new MemoryStream();
		//await image.SaveAsync(pngStream, new PngEncoder(), ct);
		//pngStream.Position = 0;

		await repository.SaveAsync(tenantKey, uploadStream, ct);

		logger.LogInformation("Brand image saved for tenant {TenantKey}", tenantKey);
	}

	public async Task<Stream?> GetAsync(string tenantKey, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tenantKey);

		return await repository.GetAsync(tenantKey, ct);
	}

	public async Task DeleteAsync(string tenantKey, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tenantKey);

		await repository.DeleteAsync(tenantKey, ct);

		logger.LogInformation("Brand image deleted for tenant {TenantKey}", tenantKey);
	}
}
