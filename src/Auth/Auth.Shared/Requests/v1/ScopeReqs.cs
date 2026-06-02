namespace Dyvenix.Auth.Shared.Requests.v1;

public class CreateScopeReq
{
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}

public class UpdateScopeReq
{
    public string Id { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}
