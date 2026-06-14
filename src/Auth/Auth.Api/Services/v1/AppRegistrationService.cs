using Dyvenix.Auth.Data.Context;
using Dyvenix.Auth.Data.Entities;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.Exceptions;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace Dyvenix.Auth.Api.Services.v1;

public class AppRegistrationService(IOpenIddictApplicationManager appManager, AuthDbContext _dbContext, ILogger<AppRegistrationService> logger) : IAppRegistrationService
{
	public async Task<AppRegistrationDto?> GetById(string id)
	{
		var app = await appManager.FindByIdAsync(id);
		return app is null ? null : await MapToDtoAsync(app);
	}

	public async Task<AppRegistrationDto?> GetByClientId(string clientId)
	{
		var app = await appManager.FindByClientIdAsync(clientId);
		return app is null ? null : await MapToDtoAsync(app);
	}

	public async Task<IReadOnlyList<AppRegistrationDto>> GetAll()
	{
		var results = new List<AppRegistrationDto>();
		await foreach (var app in appManager.ListAsync())
			results.Add(await MapToDtoAsync(app));

		return results;
	}

	public async Task<string> Create(CreateAppRegistrationReq request)
	{
		ArgumentNullException.ThrowIfNull(request);

		var descriptor = new OpenIddictApplicationDescriptor
		{
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			DisplayName = request.DisplayName,
			ConsentType = request.ConsentType
		};

		foreach (var p in request.Permissions)
			descriptor.Permissions.Add(p);

		foreach (var uri in request.RedirectUris)
			descriptor.RedirectUris.Add(new Uri(uri));

		foreach (var uri in request.PostLogoutRedirectUris)
			descriptor.PostLogoutRedirectUris.Add(new Uri(uri));

		var app = await appManager.CreateAsync(descriptor);
		var id = await appManager.GetIdAsync(app);

		// Add the tenant association
		var tenantApp = new TenantApplication
		{
			TenantId = request.TenantId,
			ApplicationId = id ?? string.Empty
		};
		await _dbContext.TenantApplication.AddAsync(tenantApp);
		await _dbContext.SaveChangesAsync();

		logger.LogInformation("Created app registration {ClientId}", request.ClientId);
		return id!;
	}

	public async Task Update(UpdateAppRegistrationReq request)
	{
		ArgumentNullException.ThrowIfNull(request);

		var app = await appManager.FindByIdAsync(request.Id)
			?? throw new NotFoundException($"AppRegistration {request.Id} not found");

		var descriptor = new OpenIddictApplicationDescriptor();
		await appManager.PopulateAsync(descriptor, app);

		if (request.DisplayName is not null)
			descriptor.DisplayName = request.DisplayName;

		if (request.ClientSecret is not null)
			descriptor.ClientSecret = request.ClientSecret;

		if (request.ConsentType is not null)
			descriptor.ConsentType = request.ConsentType;

		descriptor.Permissions.Clear();
		foreach (var p in request.Permissions)
			descriptor.Permissions.Add(p);

		descriptor.RedirectUris.Clear();
		foreach (var uri in request.RedirectUris)
			descriptor.RedirectUris.Add(new Uri(uri));

		descriptor.PostLogoutRedirectUris.Clear();
		foreach (var uri in request.PostLogoutRedirectUris)
			descriptor.PostLogoutRedirectUris.Add(new Uri(uri));

		await appManager.UpdateAsync(app, descriptor);
	}

	public async Task Delete(string id)
	{
		var app = await appManager.FindByIdAsync(id)
			?? throw new NotFoundException($"AppRegistration {id} not found");

		await appManager.DeleteAsync(app);
		logger.LogInformation("Deleted app registration {Id}", id);
	}

	private async Task<AppRegistrationDto> MapToDtoAsync(object app)
	{
		var descriptor = new OpenIddictApplicationDescriptor();
		await appManager.PopulateAsync(descriptor, app);

		var id = await appManager.GetIdAsync(app) ?? string.Empty;

		return new AppRegistrationDto(
			id,
			descriptor.ClientId ?? string.Empty,
			descriptor.DisplayName,
			descriptor.ConsentType,
			[.. descriptor.Permissions],
			[.. descriptor.RedirectUris.Select(u => u.ToString())],
			[.. descriptor.PostLogoutRedirectUris.Select(u => u.ToString())]
		);
	}
}
