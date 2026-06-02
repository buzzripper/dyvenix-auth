namespace Dyvenix.Auth.Shared.Requests.v1;

public class CreateOidcAppReq
{
    public string? ApplicationType { get; set; }
    public string ClientId { get; set; } = null!;
    public string? ClientSecret { get; set; }
    public string? DisplayName { get; set; }
    public IList<string> RedirectUris { get; set; } = [];
    public IList<string> PostLogoutRedirectUris { get; set; } = [];
}

public class UpdateOidcAppReq
{
    public string Id { get; set; } = null!;
    public string? ApplicationType { get; set; }
    public string ClientId { get; set; } = null!;
    public string? ClientSecret { get; set; }
    public string? DisplayName { get; set; }
    public IList<string> RedirectUris { get; set; } = [];
    public IList<string> PostLogoutRedirectUris { get; set; } = [];
}
