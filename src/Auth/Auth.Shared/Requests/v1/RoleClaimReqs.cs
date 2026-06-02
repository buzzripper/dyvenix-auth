namespace Dyvenix.Auth.Shared.Requests.v1;

public class CreateRoleClaimReq
{
    public string RoleId { get; set; } = null!;
    public string ClaimType { get; set; } = null!;
    public string ClaimValue { get; set; } = null!;
}

public class UpdateRoleClaimReq
{
    public int Id { get; set; }
    public string RoleId { get; set; } = null!;
    public string NewClaimType { get; set; } = null!;
    public string NewClaimValue { get; set; } = null!;
}
