using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Common.Shared.Exceptions;

namespace Dyvenix.Auth.Api.Services.v1;

public class OidcAppService(IOpenIddictApplicationManager appManager, ILogger<OidcAppService> logger) : IOidcAppService
{
    public async Task<OidcAppDto?> GetById(string id)
    {
        var app = await appManager.FindByIdAsync(id);
        return app is null ? null : await MapToDtoAsync(app);
    }

    public async Task<OidcAppDto?> GetByClientId(string clientId)
    {
        var app = await appManager.FindByClientIdAsync(clientId);
        return app is null ? null : await MapToDtoAsync(app);
    }

    public async Task<IReadOnlyList<OidcAppDto>> GetAll()
    {
        var results = new List<OidcAppDto>();
        await foreach (var app in appManager.ListAsync())
            results.Add(await MapToDtoAsync(app));

        return results;
    }

    public async Task<string> Create(CreateOidcAppReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ApplicationType = request.ApplicationType,
            ClientId = request.ClientId,
            ClientSecret = request.ClientSecret,
            DisplayName = request.DisplayName,
        };

        foreach (var uri in request.RedirectUris.Where(uri => !string.IsNullOrWhiteSpace(uri)))
            descriptor.RedirectUris.Add(new Uri(uri));

        foreach (var uri in request.PostLogoutRedirectUris.Where(uri => !string.IsNullOrWhiteSpace(uri)))
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri));

        var app = await appManager.CreateAsync(descriptor);
        var id = await appManager.GetIdAsync(app) ?? string.Empty;

        logger.LogInformation("Created OpenIddict application {ClientId}", request.ClientId);
        return id;
    }

    public async Task Update(UpdateOidcAppReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var app = await appManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException($"OpenIddict application {request.Id} not found");

        var descriptor = new OpenIddictApplicationDescriptor();
        await appManager.PopulateAsync(descriptor, app);

        descriptor.ApplicationType = request.ApplicationType;
        descriptor.ClientId = request.ClientId;
        descriptor.ClientSecret = request.ClientSecret;
        descriptor.DisplayName = request.DisplayName;

        descriptor.RedirectUris.Clear();
        foreach (var uri in request.RedirectUris.Where(uri => !string.IsNullOrWhiteSpace(uri)))
            descriptor.RedirectUris.Add(new Uri(uri));

        descriptor.PostLogoutRedirectUris.Clear();
        foreach (var uri in request.PostLogoutRedirectUris.Where(uri => !string.IsNullOrWhiteSpace(uri)))
            descriptor.PostLogoutRedirectUris.Add(new Uri(uri));

        await appManager.UpdateAsync(app, descriptor);
        logger.LogInformation("Updated OpenIddict application {Id}", request.Id);
    }

    public async Task Delete(string id)
    {
        var app = await appManager.FindByIdAsync(id)
            ?? throw new NotFoundException($"OpenIddict application {id} not found");

        await appManager.DeleteAsync(app);
        logger.LogInformation("Deleted OpenIddict application {Id}", id);
    }

    private async Task<OidcAppDto> MapToDtoAsync(object app)
    {
        var descriptor = new OpenIddictApplicationDescriptor();
        await appManager.PopulateAsync(descriptor, app);

        var id = await appManager.GetIdAsync(app) ?? string.Empty;

        return new OidcAppDto(
            id,
            descriptor.ApplicationType,
            descriptor.ClientId ?? string.Empty,
            descriptor.ClientSecret,
            descriptor.DisplayName,
            [.. descriptor.RedirectUris.Select(uri => uri.ToString())],
            [.. descriptor.PostLogoutRedirectUris.Select(uri => uri.ToString())]
        );
    }
}
