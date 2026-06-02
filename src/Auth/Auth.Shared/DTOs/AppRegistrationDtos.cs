namespace Dyvenix.Auth.Shared.DTOs;

public record AppRegistrationDto(
    string Id,
    string ClientId,
    string? DisplayName,
    string? ConsentType,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> RedirectUris,
    IReadOnlyList<string> PostLogoutRedirectUris
);
