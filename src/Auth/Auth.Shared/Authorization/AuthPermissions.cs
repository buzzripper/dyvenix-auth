// src/Auth/Auth.Shared/Authorization/AuthPermissions.cs
namespace Dyvenix.Auth.Shared.Authorization;

public static class AuthPermissions
{
    // Constants for use in RequireAuthorization()
    public const string Read = "auth:read";
    public const string Write = "auth:write";
    public const string Admin = "auth:admin";

    // Hierarchy definition
    public static readonly IReadOnlyDictionary<string, string[]> Hierarchy = 
        new Dictionary<string, string[]>
        {
            [Admin] = [Write, Read],
            [Write] = [Read],
        };
}