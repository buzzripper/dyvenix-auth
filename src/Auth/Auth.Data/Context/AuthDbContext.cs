using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Dyvenix.Auth.Data.Entities;

namespace Dyvenix.Auth.Data.Context;

public partial class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
	IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>,
	IdentityRoleClaim<string>, IdentityUserToken<string>>
{
	partial void OnModelCreatingExt(ModelBuilder builder);

	public AuthDbContext(DbContextOptions<AuthDbContext> options)
		: base(options)
	{
	}

	#region Properties

	public DbSet<Tenant> Tenant { get; set; }
	public DbSet<TenantApplication> TenantApplication { get; set; }

	#endregion

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		this.OnModelCreatingExt(modelBuilder);

		#region Tenant

		modelBuilder.Entity<Tenant>(entity =>
		{
			entity.ToTable("Tenant");
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
			entity.Property(e => e.AuthMode).IsRequired();
			entity.Property(e => e.ExternalAuthority).HasMaxLength(500);
			entity.Property(e => e.ExternalClientId).HasMaxLength(200);
			entity.Property(e => e.ExternalClientSecret).HasMaxLength(500);
			entity.Property(e => e.ADDcHost).HasMaxLength(200);
			entity.Property(e => e.ADDomain).HasMaxLength(200);
			entity.Property(e => e.ADLdapPort);
			entity.Property(e => e.ADBaseDn).HasMaxLength(200);
			entity.Property(e => e.IsActive).IsRequired();
			entity.Property(e => e.CreatedAt).IsRequired();

			entity.HasIndex(e => e.Id, "IX_Tenant_Id");
		});

		#endregion

		#region TenantApplication

		modelBuilder.Entity<TenantApplication>(entity =>
		{
			entity.ToTable("TenantApplication");
			entity.HasKey(e => new { e.TenantId, e.ClientId });
			entity.Property(e => e.TenantId).IsRequired();
			entity.Property(e => e.ClientId).IsRequired().HasMaxLength(100);

			entity.HasIndex(e => new { e.ClientId }, "IX_TenantApplication_ClientId");
		});

		#endregion

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
