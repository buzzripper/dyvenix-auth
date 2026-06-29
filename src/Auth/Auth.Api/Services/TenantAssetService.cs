using Dyvenix.Auth.Data.Context;
using Dyvenix.Auth.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dyvenix.Auth.Api.Services;

public class TenantAssetService(AuthDbContext db, ITenantAssetRepository repository, ILogger<TenantAssetService> logger) : ITenantAssetService
{
	public async Task<Stream?> Get(Guid tenantId, string path, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		return await repository.Get(tenantId, path, ct);
	}

	public async Task<Stream?> GetByTenantKey(string tenantKey, string path, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tenantKey);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		var tenantId = await db.Tenant
			.AsNoTracking()
			.Where(x => x.Key == tenantKey)
			.Select(x => (Guid?)x.Id)
			.SingleOrDefaultAsync(ct);

		if (tenantId is null)
			return null;

		return await repository.Get(tenantId.Value, path, ct);
	}

	public async Task Save(Guid tenantId, string path, Stream stream, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await repository.Save(tenantId, path, stream, ct);

		logger.LogInformation("Asset saved for tenant {TenantId} at {Path}", tenantId, path);
	}

	public async Task Delete(Guid tenantId, string path, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await repository.Delete(tenantId, path, ct);

		logger.LogInformation("Asset deleted for tenant {TenantId} at {Path}", tenantId, path);
	}
}
