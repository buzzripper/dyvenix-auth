
namespace Dyvenix.Auth.Shared.Requests.v1;

public class UpdateNameReq
{
	public Guid Id { get; set; }

	// Required properties
	public string Name { get; set; } = null!;
}
