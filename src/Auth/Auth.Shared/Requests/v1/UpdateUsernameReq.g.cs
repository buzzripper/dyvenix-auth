
namespace Dyvenix.Auth.Shared.Requests.v1;

public class UpdateUsernameReq
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
}
