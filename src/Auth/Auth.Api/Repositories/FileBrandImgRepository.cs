using Dyvenix.Auth.Api.Config;
using Dyvenix.Auth.Shared.Contracts;
using Microsoft.Extensions.Options;

namespace Dyvenix.Auth.Api.Repositories;

public class FileBrandImgRepository(IOptions<BrandImgOptions> options) : IBrandImgRepository
{
	private readonly string _basePath = options.Value.File.BasePath;

	public async Task SaveAsync(string tenantKey, Stream imageStream, CancellationToken ct = default)
	{
		Directory.CreateDirectory(_basePath);
		var filePath = GetFilePath(tenantKey);

		await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
		await imageStream.CopyToAsync(fileStream, ct);
	}

	public Task<Stream?> GetAsync(string tenantKey, CancellationToken ct = default)
	{
		var filePath = GetFilePath(tenantKey);

		if (!File.Exists(filePath))
			return Task.FromResult<Stream?>(null);

		Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		return Task.FromResult<Stream?>(stream);
	}

	public Task DeleteAsync(string tenantKey, CancellationToken ct = default)
	{
		var filePath = GetFilePath(tenantKey);

		if (File.Exists(filePath))
			File.Delete(filePath);

		return Task.CompletedTask;
	}

	private string GetFilePath(string tenantKey) => Path.Combine(_basePath, $"{tenantKey}.png");
}
