using Dyvenix.Auth.Data.Context;
using Dyvenix.Auth.Data.Entities;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dyvenix.Auth.Api.Services.v1;

public partial class TenantService : ITenantService
{
	private readonly ILogger<TenantService> _logger;
	private readonly AuthDbContext _db;

	public TenantService(AuthDbContext db, ILogger<TenantService> logger)
	{
		_db = db;
		_logger = logger;
	}

	public async Task<TenantDto?> GetById(Guid id)
	{
		return await _db.Tenant
			.AsNoTracking()
			.Where(x => x.Id == id)
			.Select(MapToDto())
			.SingleOrDefaultAsync();
	}

	public async Task<TenantDto?> GetBySlug(string slug)
	{
		if (string.IsNullOrWhiteSpace(slug))
			return null;

		return await _db.Tenant
			.AsNoTracking()
			.Where(x => x.Slug == slug)
			.Select(MapToDto())
			.SingleOrDefaultAsync();
	}

	public async Task<IReadOnlyList<TenantDto>> GetAll()
	{
		return await _db.Tenant
			.AsNoTracking()
			.OrderBy(x => x.Name)
			.Select(MapToDto())
			.ToListAsync();
	}

	public async Task<Guid> Create(CreateTenantReq request)
	{
		ArgumentNullException.ThrowIfNull(request);

		var tenant = new Tenant
		{
			Id = request.Id,
			Name = request.Name,
			Slug = request.Slug,
			AuthMode = request.AuthMode,
			ExternalAuthority = request.ExternalAuthority,
			ExternalClientId = request.ExternalClientId,
			ExternalClientSecret = request.ExternalClientSecret,
			ADDcHost = request.ADDcHost,
			ADDomain = request.ADDomain,
			ADLdapPort = request.ADLdapPort,
			ADBaseDn = request.ADBaseDn,
			IsActive = request.IsActive,
			CreatedAt = request.CreatedAt,
		};

		_db.Tenant.Add(tenant);
		await _db.SaveChangesAsync();

		_logger.LogInformation("Created tenant {TenantId}", tenant.Id);
		return tenant.Id;
	}

	public async Task Update(UpdateTenantReq request)
	{
		ArgumentNullException.ThrowIfNull(request);

		var rowsAffected = await _db.Tenant
			.Where(x => x.Id == request.Id)
			.ExecuteUpdateAsync(setters => setters
				.SetProperty(x => x.Name, request.Name)
				.SetProperty(x => x.Slug, request.Slug)
				.SetProperty(x => x.AuthMode, request.AuthMode)
				.SetProperty(x => x.ExternalAuthority, request.ExternalAuthority)
				.SetProperty(x => x.ExternalClientId, request.ExternalClientId)
				.SetProperty(x => x.ExternalClientSecret, request.ExternalClientSecret)
				.SetProperty(x => x.ADDcHost, request.ADDcHost)
				.SetProperty(x => x.ADDomain, request.ADDomain)
				.SetProperty(x => x.ADLdapPort, request.ADLdapPort)
				.SetProperty(x => x.ADBaseDn, request.ADBaseDn)
				.SetProperty(x => x.IsActive, request.IsActive)
				.SetProperty(x => x.CreatedAt, request.CreatedAt));

		if (rowsAffected == 0)
			throw new NotFoundException($"Tenant {request.Id} not found");

		_logger.LogInformation("Updated tenant {TenantId}", request.Id);
	}

	public async Task Delete(Guid id)
	{
		var rowsAffected = await _db.Tenant.Where(x => x.Id == id).ExecuteDeleteAsync();

		if (rowsAffected == 0)
			throw new NotFoundException($"Tenant {id} not found");

		_logger.LogInformation("Deleted tenant {TenantId}", id);
	}

	private static System.Linq.Expressions.Expression<Func<Tenant, TenantDto>> MapToDto()
	{
		return tenant => new TenantDto(
			tenant.Id,
			tenant.Name,
			tenant.Slug,
			tenant.AuthMode,
			tenant.ExternalAuthority,
			tenant.ExternalClientId,
			tenant.ExternalClientSecret,
			tenant.ADDcHost,
			tenant.ADDomain,
			tenant.ADLdapPort,
			tenant.ADBaseDn,
			tenant.IsActive,
			tenant.CreatedAt);
	}
}
