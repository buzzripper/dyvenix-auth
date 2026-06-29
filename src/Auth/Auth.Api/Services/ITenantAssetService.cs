namespace Dyvenix.Auth.Api.Services;

public interface ITenantAssetService
{
	Task<Stream?> Get(Guid tenantId, string path, CancellationToken ct = default);
	Task<Stream?> GetByTenantKey(string tenantKey, string path, CancellationToken ct = default);
	Task Save(Guid tenantId, string path, Stream stream, CancellationToken ct = default);
	Task Delete(Guid tenantId, string path, CancellationToken ct = default);
}
