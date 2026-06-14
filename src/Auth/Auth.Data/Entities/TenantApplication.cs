
namespace Dyvenix.Auth.Data.Entities;

public class TenantApplication
{
	public Guid TenantId { get; set; }
	public string ApplicationId { get; set; } = null!;
}
