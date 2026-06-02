namespace Dyvenix.Auth.Shared.Requests.v1;

public class CreateRoleReq
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
}

public class UpdateRoleReq
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}
