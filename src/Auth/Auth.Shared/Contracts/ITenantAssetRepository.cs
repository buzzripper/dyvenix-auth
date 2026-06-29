namespace Dyvenix.Auth.Shared.Contracts;

public interface ITenantAssetRepository
{
	Task Save(Guid tenantId, string path, Stream stream, CancellationToken ct = default);
	Task<Stream?> Get(Guid tenantId, string path, CancellationToken ct = default);
	Task Delete(Guid tenantId, string path, CancellationToken ct = default);
}
