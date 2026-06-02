namespace Dyvenix.Auth.Shared.DTOs;

public record ScopeDto(
    string Id,
    string Name,
    string? DisplayName,
    string? Description
);
