namespace Dyvenix.Auth.Shared.Requests.v1;

public class CreateUserClaimReq
{
    public string UserId { get; set; } = null!;
    public string ClaimType { get; set; } = null!;
    public string ClaimValue { get; set; } = null!;
}

public class UpdateUserClaimReq
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string NewClaimType { get; set; } = null!;
    public string NewClaimValue { get; set; } = null!;
}
