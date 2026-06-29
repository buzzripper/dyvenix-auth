using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Dyvenix.Auth.Api.Config;
using Dyvenix.Auth.Shared.Contracts;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Dyvenix.Auth.Api.Repositories;

public class AzureBlobTenantAssetRepository : ITenantAssetRepository
{
	private readonly BlobContainerClient _containerClient;
	private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

	public AzureBlobTenantAssetRepository(IOptions<TenantAssetOptions> options)
	{
		var config = options.Value.AzureBlob;
		var serviceClient = new BlobServiceClient(config.ConnectionString);
		_containerClient = serviceClient.GetBlobContainerClient(config.ContainerName);
	}

	public async Task Save(Guid tenantId, string path, Stream stream, CancellationToken ct = default)
	{
		await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

		var blobClient = _containerClient.GetBlobClient(GetBlobName(tenantId, path));
		var contentType = GetContentType(path);
		await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
	}

	public async Task<Stream?> Get(Guid tenantId, string path, CancellationToken ct = default)
	{
		var blobClient = _containerClient.GetBlobClient(GetBlobName(tenantId, path));

		if (!await blobClient.ExistsAsync(ct))
			return null;

		var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
		return response.Value.Content;
	}

	public async Task Delete(Guid tenantId, string path, CancellationToken ct = default)
	{
		var blobClient = _containerClient.GetBlobClient(GetBlobName(tenantId, path));
		await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
	}

	private static string GetBlobName(Guid tenantId, string path) => $"{tenantId}/{path}";

	private static string GetContentType(string path)
	{
		return _contentTypeProvider.TryGetContentType(path, out var contentType)
			? contentType
			: "application/octet-stream";
	}
}
