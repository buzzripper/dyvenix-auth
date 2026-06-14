namespace Dyvenix.Auth.Shared.Requests.v1;

public class CreateAppRegistrationReq
{
	public string ClientId { get; set; } = null!;
	public string? ClientSecret { get; set; }
	public string? DisplayName { get; set; }
	public string? ConsentType { get; set; }
	public IList<string> Permissions { get; set; } = [];
	public IList<string> RedirectUris { get; set; } = [];
	public IList<string> PostLogoutRedirectUris { get; set; } = [];
	public Guid ApplicationId { get; set; }
	public Guid TenantId { get; set; }
}

public class UpdateAppRegistrationReq
{
	public Guid ApplicationId { get; set; }
	public string Id { get; set; } = null!;
	public string? DisplayName { get; set; }
	public string? ClientSecret { get; set; }
	public string? ConsentType { get; set; }
	public IList<string> Permissions { get; set; } = [];
	public IList<string> RedirectUris { get; set; } = [];
	public IList<string> PostLogoutRedirectUris { get; set; } = [];
}
