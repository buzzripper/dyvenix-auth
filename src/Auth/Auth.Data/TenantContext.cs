using Dyvenix.Auth.Data.Entities;

namespace Dyvenix.Auth.Data;

public interface ITenantContext
{
	Guid TenantId { get; }
	string? TenantKey { get; }
	Tenant? Tenant { get; }

	void Set(Tenant tenant);
}

public class TenantContext : ITenantContext
{
	public Tenant? Tenant { get; private set; }
	public Guid TenantId => Tenant?.Id ?? Guid.Empty;
	public string? TenantKey => Tenant?.Key;

	public void Set(Tenant tenant) => Tenant = tenant;
}
