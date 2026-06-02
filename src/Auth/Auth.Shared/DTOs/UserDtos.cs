namespace Dyvenix.Auth.Shared.DTOs;

public record UserDto(
    string Id,
    Guid TenantId,
    string UserName,
    string Email,
    string? PhoneNumber,
    bool EmailConfirmed,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    bool TwoFactorEnabled
);

public record UserSummaryDto(
    string Id,
    Guid TenantId,
    string UserName,
    string Email
);
