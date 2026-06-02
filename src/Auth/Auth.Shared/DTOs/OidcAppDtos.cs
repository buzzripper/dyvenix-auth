namespace Dyvenix.Auth.Shared.DTOs;

public record OidcAppDto(
    string Id,
    string? ApplicationType,
    string ClientId,
    string? ClientSecret,
    string? DisplayName,
    IReadOnlyList<string> RedirectUris,
    IReadOnlyList<string> PostLogoutRedirectUris
);
