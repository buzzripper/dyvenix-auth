namespace Dyvenix.Auth.Shared.Contracts;

public interface IBrandImgRepository
{
	Task SaveAsync(string tenantKey, Stream imageStream, CancellationToken ct = default);
	Task<Stream?> GetAsync(string tenantKey, CancellationToken ct = default);
	Task DeleteAsync(string tenantKey, CancellationToken ct = default);
}
