using Dyvenix.Auth.Shared.DTOs;

namespace Dyvenix.Auth.Shared.Requests.v1;

public class CreateTenantReq
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Key { get; set; } = null!;
    public AuthMode AuthMode { get; set; }
    public string? ExternalAuthority { get; set; }
    public string? ExternalClientId { get; set; }
    public string? ExternalClientSecret { get; set; }
    public string? ADDcHost { get; set; }
    public string? ADDomain { get; set; }
    public int? ADLdapPort { get; set; }
    public string? ADBaseDn { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateTenantReq
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Key { get; set; } = null!;
    public AuthMode AuthMode { get; set; }
    public string? ExternalAuthority { get; set; }
    public string? ExternalClientId { get; set; }
    public string? ExternalClientSecret { get; set; }
    public string? ADDcHost { get; set; }
    public string? ADDomain { get; set; }
    public int? ADLdapPort { get; set; }
    public string? ADBaseDn { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
