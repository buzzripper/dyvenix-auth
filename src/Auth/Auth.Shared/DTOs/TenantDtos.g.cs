
namespace Dyvenix.Auth.Shared.DTOs;

public record TenantDto(
    Guid Id,
    string Name,
    string Key,
    AuthMode AuthMode,
    string? ExternalAuthority,
    string? ExternalClientId,
    string? ExternalClientSecret,
    string? ADDcHost,
    string? ADDomain,
    int? ADLdapPort,
    string? ADBaseDn,
    bool IsActive,
    DateTime CreatedAt
);
