using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Dyvenix.Auth.Api.Config;
using Dyvenix.Auth.Shared.Contracts;
using Microsoft.Extensions.Options;

namespace Dyvenix.Auth.Api.Repositories;

public class AzureBlobBrandImgRepository : IBrandImgRepository
{
	private readonly BlobContainerClient _containerClient;

	public AzureBlobBrandImgRepository(IOptions<BrandImgOptions> options)
	{
		var config = options.Value.AzureBlob;
		var serviceClient = new BlobServiceClient(config.ConnectionString);
		_containerClient = serviceClient.GetBlobContainerClient(config.ContainerName);
	}

	public async Task SaveAsync(string tenantKey, Stream imageStream, CancellationToken ct = default)
	{
		await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);

		var blobClient = _containerClient.GetBlobClient(GetBlobName(tenantKey));
		await blobClient.UploadAsync(imageStream, new BlobHttpHeaders { ContentType = "image/png" }, cancellationToken: ct);
	}

	public async Task<Stream?> GetAsync(string tenantKey, CancellationToken ct = default)
	{
		var blobClient = _containerClient.GetBlobClient(GetBlobName(tenantKey));

		if (!await blobClient.ExistsAsync(ct))
			return null;

		var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
		return response.Value.Content;
	}

	public async Task DeleteAsync(string tenantKey, CancellationToken ct = default)
	{
		var blobClient = _containerClient.GetBlobClient(GetBlobName(tenantKey));
		await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
	}

	private static string GetBlobName(string tenantKey) => $"{tenantKey}.png";
}
