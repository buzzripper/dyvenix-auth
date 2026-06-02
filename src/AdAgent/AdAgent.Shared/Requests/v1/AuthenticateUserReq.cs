namespace Dyvenix.AdAgent.Shared.Requests.v1;

public class AuthenticateUserReq
{
	public string UserUpnOrDomainUser { get; set; } = null!;
	public string Password { get; set; } = null!;
}
