using Microsoft.AspNetCore.Identity;

namespace Dyvenix.Auth.Data;

public class ApplicationRole : IdentityRole
{
    public Guid TenantId { get; set; }
}
