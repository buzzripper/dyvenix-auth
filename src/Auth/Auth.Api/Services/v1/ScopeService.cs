using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Common.Shared.Exceptions;

namespace Dyvenix.Auth.Api.Services.v1;

public class ScopeService(IOpenIddictScopeManager scopeManager, ILogger<ScopeService> logger) : IScopeService
{
    public async Task<ScopeDto?> GetById(string id)
    {
        var scope = await scopeManager.FindByIdAsync(id);
        return scope is null ? null : await MapToDtoAsync(scope);
    }

    public async Task<ScopeDto?> GetByName(string name)
    {
        var scope = await scopeManager.FindByNameAsync(name);
        return scope is null ? null : await MapToDtoAsync(scope);
    }

    public async Task<IReadOnlyList<ScopeDto>> GetAll()
    {
        var results = new List<ScopeDto>();
        await foreach (var scope in scopeManager.ListAsync())
            results.Add(await MapToDtoAsync(scope));

        return results;
    }

    public async Task<string> Create(CreateScopeReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var descriptor = new OpenIddictScopeDescriptor
        {
            Name = request.Name,
            DisplayName = request.DisplayName,
            Description = request.Description
        };

        var scope = await scopeManager.CreateAsync(descriptor);
        var id = await scopeManager.GetIdAsync(scope);

        logger.LogInformation("Created scope {Name}", request.Name);
        return id!;
    }

    public async Task Update(UpdateScopeReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var scope = await scopeManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException($"Scope {request.Id} not found");

        var descriptor = new OpenIddictScopeDescriptor();
        await scopeManager.PopulateAsync(descriptor, scope);

        if (request.DisplayName is not null)
            descriptor.DisplayName = request.DisplayName;

        if (request.Description is not null)
            descriptor.Description = request.Description;

        await scopeManager.UpdateAsync(scope, descriptor);
    }

    public async Task Delete(string id)
    {
        var scope = await scopeManager.FindByIdAsync(id)
            ?? throw new NotFoundException($"Scope {id} not found");

        await scopeManager.DeleteAsync(scope);
        logger.LogInformation("Deleted scope {Id}", id);
    }

    private async Task<ScopeDto> MapToDtoAsync(object scope)
    {
        var id = await scopeManager.GetIdAsync(scope) ?? string.Empty;
        var name = await scopeManager.GetNameAsync(scope) ?? string.Empty;
        var displayName = await scopeManager.GetDisplayNameAsync(scope);
        var description = await scopeManager.GetDescriptionAsync(scope);

        return new ScopeDto(id, name, displayName, description);
    }
}
