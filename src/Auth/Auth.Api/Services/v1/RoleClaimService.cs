using System.Security.Claims;
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

public class RoleClaimService(RoleManager<ApplicationRole> roleManager, AuthDbContext db, ILogger<RoleClaimService> logger) : IRoleClaimService
{
    public async Task<RoleClaimDto?> GetById(int id)
    {
        var claim = await db.RoleClaims.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return claim is null ? null : new RoleClaimDto(claim.Id, claim.RoleId, claim.ClaimType!, claim.ClaimValue!);
    }

    public async Task<IReadOnlyList<RoleClaimDto>> GetAllByRole(string roleId)
    {
        return await db.RoleClaims
            .AsNoTracking()
            .Where(c => c.RoleId == roleId)
            .Select(c => new RoleClaimDto(c.Id, c.RoleId, c.ClaimType!, c.ClaimValue!))
            .ToListAsync();
    }

    public async Task Create(CreateRoleClaimReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var role = await roleManager.FindByIdAsync(request.RoleId)
            ?? throw new NotFoundException($"Role {request.RoleId} not found");

        var result = await roleManager.AddClaimAsync(role, new Claim(request.ClaimType, request.ClaimValue));
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);

        logger.LogInformation("Added claim {ClaimType} to role {RoleId}", request.ClaimType, request.RoleId);
    }

    public async Task Update(UpdateRoleClaimReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await db.RoleClaims.FirstOrDefaultAsync(c => c.Id == request.Id)
            ?? throw new NotFoundException($"RoleClaim {request.Id} not found");

        var role = await roleManager.FindByIdAsync(existing.RoleId)
            ?? throw new NotFoundException($"Role {existing.RoleId} not found");

        var oldClaim = new Claim(existing.ClaimType!, existing.ClaimValue!);
        var newClaim = new Claim(request.NewClaimType, request.NewClaimValue);

        var removeResult = await roleManager.RemoveClaimAsync(role, oldClaim);
        if (!removeResult.Succeeded)
            throw new ValidationException(string.Join("; ", removeResult.Errors.Select(e => e.Description)), []);

        var addResult = await roleManager.AddClaimAsync(role, newClaim);
        if (!addResult.Succeeded)
            throw new ValidationException(string.Join("; ", addResult.Errors.Select(e => e.Description)), []);
    }

    public async Task Delete(int id)
    {
        var existing = await db.RoleClaims.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException($"RoleClaim {id} not found");

        var role = await roleManager.FindByIdAsync(existing.RoleId)
            ?? throw new NotFoundException($"Role {existing.RoleId} not found");

        var claim = new Claim(existing.ClaimType!, existing.ClaimValue!);
        var result = await roleManager.RemoveClaimAsync(role, claim);
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);
    }
}
