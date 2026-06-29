using Dyvenix.Auth.Api.Config;
using Dyvenix.Auth.Shared.Contracts;
using Microsoft.Extensions.Options;

namespace Dyvenix.Auth.Api.Repositories;

public class FileTenantAssetRepository(IOptions<TenantAssetOptions> options) : ITenantAssetRepository
{
	private readonly string _basePath = options.Value.File.BasePath;

	public Task<Stream?> Get(Guid tenantId, string path, CancellationToken ct = default)
	{
		var filePath = GetFilePath(tenantId, path);

		if (!File.Exists(filePath))
			return Task.FromResult<Stream?>(null);

		Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		return Task.FromResult<Stream?>(stream);
	}

	public async Task Save(Guid tenantId, string path, Stream stream, CancellationToken ct = default)
	{
		var filePath = GetFilePath(tenantId, path);
		Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

		await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
		await stream.CopyToAsync(fileStream, ct);
	}


	public Task Delete(Guid tenantId, string path, CancellationToken ct = default)
	{
		var filePath = GetFilePath(tenantId, path);

		if (File.Exists(filePath))
			File.Delete(filePath);

		return Task.CompletedTask;
	}

	private string GetFilePath(Guid tenantId, string path)
	{
		var normalizedPath = path.Replace('/', Path.DirectorySeparatorChar);
		return Path.Combine(_basePath, tenantId.ToString(), normalizedPath);
	}
}
