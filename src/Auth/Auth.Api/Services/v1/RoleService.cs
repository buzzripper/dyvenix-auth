using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dyvenix.Auth.Data;
using Dyvenix.Auth.Data.Context;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Common.Shared.Exceptions;

namespace Dyvenix.Auth.Api.Services.v1;

public class RoleService(RoleManager<ApplicationRole> roleManager, AuthDbContext db, ILogger<RoleService> logger) : IRoleService
{
    public async Task<RoleDto?> GetById(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        return role is null ? null : MapToDto(role);
    }

    public async Task<RoleDto?> GetByName(string name)
    {
        var role = await roleManager.FindByNameAsync(name);
        return role is null ? null : MapToDto(role);
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllByTenant(Guid tenantId)
    {
        return await db.Roles
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Select(r => new RoleDto(r.Id, r.TenantId, r.Name!))
            .ToListAsync();
    }

    public async Task<string> Create(CreateRoleReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var role = new ApplicationRole
        {
            TenantId = request.TenantId,
            Name = request.Name
        };

        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);

        logger.LogInformation("Created role {RoleName} for tenant {TenantId}", role.Name, role.TenantId);
        return role.Id;
    }

    public async Task Update(UpdateRoleReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var role = await roleManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException($"Role {request.Id} not found");

        role.Name = request.Name;

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);
    }

    public async Task Delete(string id)
    {
        var role = await roleManager.FindByIdAsync(id)
            ?? throw new NotFoundException($"Role {id} not found");

        var result = await roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);
    }

    private static RoleDto MapToDto(ApplicationRole r) => new(r.Id, r.TenantId, r.Name!);
}
