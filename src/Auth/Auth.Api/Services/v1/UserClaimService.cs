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

public class UserClaimService(UserManager<ApplicationUser> userManager, AuthDbContext db, ILogger<UserClaimService> logger) : IUserClaimService
{
    public async Task<UserClaimDto?> GetById(int id)
    {
        var claim = await db.UserClaims.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return claim is null ? null : new UserClaimDto(claim.Id, claim.UserId, claim.ClaimType!, claim.ClaimValue!);
    }

    public async Task<IReadOnlyList<UserClaimDto>> GetAllByUser(string userId)
    {
        return await db.UserClaims
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new UserClaimDto(c.Id, c.UserId, c.ClaimType!, c.ClaimValue!))
            .ToListAsync();
    }

    public async Task Create(CreateUserClaimReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException($"User {request.UserId} not found");

        var result = await userManager.AddClaimAsync(user, new Claim(request.ClaimType, request.ClaimValue));
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);

        logger.LogInformation("Added claim {ClaimType} to user {UserId}", request.ClaimType, request.UserId);
    }

    public async Task Update(UpdateUserClaimReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await db.UserClaims.FirstOrDefaultAsync(c => c.Id == request.Id)
            ?? throw new NotFoundException($"UserClaim {request.Id} not found");

        var user = await userManager.FindByIdAsync(existing.UserId)
            ?? throw new NotFoundException($"User {existing.UserId} not found");

        var oldClaim = new Claim(existing.ClaimType!, existing.ClaimValue!);
        var newClaim = new Claim(request.NewClaimType, request.NewClaimValue);

        var result = await userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);
    }

    public async Task Delete(int id)
    {
        var existing = await db.UserClaims.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException($"UserClaim {id} not found");

        var user = await userManager.FindByIdAsync(existing.UserId)
            ?? throw new NotFoundException($"User {existing.UserId} not found");

        var claim = new Claim(existing.ClaimType!, existing.ClaimValue!);
        var result = await userManager.RemoveClaimAsync(user, claim);
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);
    }
}
