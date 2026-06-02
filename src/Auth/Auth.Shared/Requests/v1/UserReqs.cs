namespace Dyvenix.Auth.Shared.Requests.v1;

public class CreateUserReq
{
    public Guid TenantId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}

public class UpdateUserReq
{
    public string Id { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}
