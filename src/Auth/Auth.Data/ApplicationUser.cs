using Microsoft.AspNetCore.Identity;

namespace Dyvenix.Auth.Data;

public class ApplicationUser : IdentityUser
{
    public Guid TenantId { get; set; }
}
