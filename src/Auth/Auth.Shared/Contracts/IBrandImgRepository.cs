namespace Dyvenix.Auth.Shared.Contracts;

public interface IBrandImgRepository
{
	Task SaveAsync(string tenantSlug, Stream imageStream, CancellationToken ct = default);
	Task<Stream?> GetAsync(string tenantSlug, CancellationToken ct = default);
	Task DeleteAsync(string tenantSlug, CancellationToken ct = default);
}
