using Dyvenix.Auth.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dyvenix.Auth.Data.Context;

public partial class AuthDbContext
{
	private readonly ITenantContext? _tenantContext;

	/// <summary>
	/// Constructor for derived contexts (e.g. AuthServerDbContext in Auth.Server).
	/// </summary>
	public AuthDbContext(DbContextOptions<AuthDbContext> options, ITenantContext? tenantContext = null)
		: base(options)
	{
		_tenantContext = tenantContext;
	}

	partial void OnModelCreatingExt(ModelBuilder builder)
	{
		builder.Entity<ApplicationUser>(b =>
		{
			b.HasIndex(u => new { u.TenantId, u.Email });
			b.HasOne<Tenant>().WithMany().HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Restrict);

			// Global query filter: all Identity queries are scoped to the current tenant
			b.HasQueryFilter(u => _tenantContext == null
				|| _tenantContext.TenantId == Guid.Empty
				|| u.TenantId == _tenantContext.TenantId);
		});

		builder.Entity<ApplicationRole>(b =>
		{
			b.HasOne<Tenant>().WithMany().HasForeignKey(r => r.TenantId).OnDelete(DeleteBehavior.Restrict);
			b.HasIndex(r => new { r.TenantId, r.NormalizedName }).IsUnique();

			// Global query filter: scope role queries to the current tenant
			b.HasQueryFilter(r => _tenantContext == null
				|| _tenantContext.TenantId == Guid.Empty
				|| r.TenantId == _tenantContext.TenantId);
		});
	}
}
