namespace Dyvenix.Auth.Shared.DTOs;

public record UserClaimDto(
    int Id,
    string UserId,
    string ClaimType,
    string ClaimValue
);
