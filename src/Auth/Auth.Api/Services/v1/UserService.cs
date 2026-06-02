using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Dyvenix.Auth.Data;
using Dyvenix.Auth.Data.Context;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Common.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Dyvenix.Auth.Api.Services.v1;

public class UserService(UserManager<ApplicationUser> userManager, AuthDbContext db, ILogger<UserService> logger) : IUserService
{
    public async Task<UserDto?> GetById(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByEmail(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return user is null ? null : MapToDto(user);
    }

    public async Task<IReadOnlyList<UserSummaryDto>> GetAllByTenant(Guid tenantId)
    {
        return await db.Users
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId)
            .Select(u => new UserSummaryDto(u.Id, u.TenantId, u.UserName!, u.Email!))
            .ToListAsync();
    }

    public async Task<string> Create(CreateUserReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = new ApplicationUser
        {
            TenantId = request.TenantId,
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);

        logger.LogInformation("Created user {UserName} for tenant {TenantId}", user.UserName, user.TenantId);
        return user.Id;
    }

    public async Task Update(UpdateUserReq request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await userManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException($"User {request.Id} not found");

        if (request.Email is not null)
        {
            var token = await userManager.GenerateChangeEmailTokenAsync(user, request.Email);
            var result = await userManager.ChangeEmailAsync(user, request.Email, token);
            if (!result.Succeeded)
                throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);
        }

        if (request.PhoneNumber is not null)
            user.PhoneNumber = request.PhoneNumber;

        if (request.LockoutEnabled.HasValue)
            await userManager.SetLockoutEnabledAsync(user, request.LockoutEnabled.Value);

        if (request.LockoutEnd.HasValue)
            await userManager.SetLockoutEndDateAsync(user, request.LockoutEnd.Value);

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new ValidationException(string.Join("; ", updateResult.Errors.Select(e => e.Description)), []);
    }

    public async Task Delete(string id)
    {
        var user = await userManager.FindByIdAsync(id)
            ?? throw new NotFoundException($"User {id} not found");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new ValidationException(string.Join("; ", result.Errors.Select(e => e.Description)), []);
    }

    private static UserDto MapToDto(ApplicationUser u) => new(
        u.Id,
        u.TenantId,
        u.UserName!,
        u.Email!,
        u.PhoneNumber,
        u.EmailConfirmed,
        u.LockoutEnabled,
        u.LockoutEnd,
        u.TwoFactorEnabled
    );
}
