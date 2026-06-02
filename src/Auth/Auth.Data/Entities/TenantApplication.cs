
namespace Dyvenix.Auth.Data.Entities;

public class TenantApplication
{
	public Guid TenantId { get; set; }
	public string ClientId { get; set; } = null!;
}
