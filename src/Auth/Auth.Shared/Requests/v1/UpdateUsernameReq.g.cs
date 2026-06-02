
namespace Dyvenix.Auth.Shared.Requests.v1;

public class UpdateUsernameReq
{
	public Guid Id { get; set; }

	// Required properties
	public string Name { get; set; }
}
