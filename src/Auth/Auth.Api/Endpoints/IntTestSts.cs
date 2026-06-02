using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Add this using directive
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dyvenix.Auth.Api.Endpoints;

public record TestTokenRequest(
    string UserId,
    string Organization,
    List<string> Roles
);

public static class IntTestSts
{
    public static IEndpointRouteBuilder MapStsEndpoints(this IEndpointRouteBuilder app)
    {
        var env = app.ServiceProvider.GetService<IHostEnvironment>();
        if (env is not null && (env.IsDevelopment() || env.IsEnvironment("Test")))
        {
            app.MapPost("/test/token", (TestTokenRequest req) =>
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, req.UserId ?? "dev-user"),
                    new Claim("uid", req.UserId ?? "dev-user"),
                    new Claim("org", req.Organization ?? "dev-org"),
                };

                if (req.Roles is not null)
                {
                    foreach (var role in req.Roles)
                        claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("super-secret-test-key-123456"));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "https://dev.local",
                    audience: "dyvenix-api",
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds);

                return Results.Ok(new
                {
                    access_token = new JwtSecurityTokenHandler().WriteToken(token)

                });
            });
        }

        return app;
    }
}