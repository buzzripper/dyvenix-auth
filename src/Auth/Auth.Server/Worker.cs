using Dyvenix.Auth.Data;
using Dyvenix.Auth.Data.Context;
using Dyvenix.Auth.Data.Entities;
using Dyvenix.Auth.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Dyvenix.Auth.Server
{
    public class Worker(IServiceProvider serviceProvider, ILogger<Worker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                await context.Database.EnsureCreatedAsync(stoppingToken);

                await SeedTenantsAsync(context);
                await SeedTestUsersAsync(scope.ServiceProvider);
                await RegisterExternalSchemesAsync(scope.ServiceProvider, context);
                await RegisterApplicationsAsync(scope.ServiceProvider, context);
                await RegisterScopesAsync(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Worker failed during startup seeding");
            }

            static async Task RegisterApplicationsAsync(IServiceProvider provider, AuthDbContext context)
            {
                var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

                // Build redirect URIs dynamically from tenant keys
                var tenantKeys = await context.Tenant
                    .Select(t => t.Key)
                    .ToListAsync();

                var redirectUris = new HashSet<Uri> { new("https://logixpro.local:5001/signin-oidc") };
                var postLogoutUris = new HashSet<Uri> { new("https://logixpro.local:5001/signout-callback-oidc") };

                foreach (var key in tenantKeys)
                {
                    redirectUris.Add(new Uri($"https://{key}.logixpro.local:5001/signin-oidc"));
                    postLogoutUris.Add(new Uri($"https://{key}.logixpro.local:5001/signout-callback-oidc"));
                }

                // Portal BFF (confidential client, authorization code + refresh token)
                // Delete and recreate to ensure redirect URIs stay in sync with tenant list.
                var portalBff = await manager.FindByClientIdAsync("portal-bff");
                if (portalBff != null)
                {
                    await manager.DeleteAsync(portalBff);
                }

                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "portal-bff",
                    ClientSecret = "portal-bff-secret",
                    ConsentType = ConsentTypes.Implicit,
                    DisplayName = "Portal BFF",
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.EndSession,
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Revocation,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        Permissions.Prefixes.Scope + "app1-api"
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                };

                foreach (var uri in redirectUris) descriptor.RedirectUris.Add(uri);
                foreach (var uri in postLogoutUris) descriptor.PostLogoutRedirectUris.Add(uri);

                await manager.CreateAsync(descriptor);

                // API resource server (for introspection if needed)
                if (await manager.FindByClientIdAsync("app1Apis") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "app1Apis",
                        ClientSecret = "app1Apis-secret",
                        Permissions =
                        {
                            Permissions.Endpoints.Introspection
                        }
                    });
                }

                // Integration API resource server
                if (await manager.FindByClientIdAsync("integrationApis") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "integrationApis",
                        ClientSecret = "integrationApis-secret",
                        Permissions =
                        {
                            Permissions.Endpoints.Introspection
                        }
                    });
                }

                // ── Vendor clients (client credentials → integration-api scope) ──────────
                // Delete and recreate on every startup so that permission changes in the
                // Properties column take effect without a manual DB edit.
                var vendorAcme = await manager.FindByClientIdAsync("vendor-acme");
                if (vendorAcme != null)
                    await manager.DeleteAsync(vendorAcme);

                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "vendor-acme",
                    ClientSecret = "vendor-acme-secret",
                    DisplayName = "Acme Vendor",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.Prefixes.Scope + "integration-api"
                    },
                    Properties =
                    {
                        ["perm"] = JsonSerializer.SerializeToElement(new[] { "integration:read" })
                    }
                });
            }

            static async Task RegisterScopesAsync(IServiceProvider provider)
            {
                var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

                if (await manager.FindByNameAsync("app1-api") is null)
                {
                    await manager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        DisplayName = "Dyvenix API access",
                        Name = "app1-api",
                        Resources =
                        {
                            "app1Apis"
                        }
                    });
                }

                if (await manager.FindByNameAsync("integration-api") is null)
                {
                    await manager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        DisplayName = "Integration API access",
                        Name = "integration-api",
                        Resources =
                        {
                            "integrationApis"
                        }
                    });
                }
            }

            static async Task SeedTenantsAsync(AuthDbContext context)
            {
                var acmeId = Guid.Parse("A1000000-0000-0000-0000-000000000001");
                var contosoId = Guid.Parse("A1000000-0000-0000-0000-000000000002");
                var fabrikamId = Guid.Parse("A1000000-0000-0000-0000-000000000003");

                if (!await context.Tenant.AnyAsync(t => t.Id == acmeId))
                {
                    context.Tenant.Add(new Tenant
                    {
                        Id = acmeId,
                        Name = "Acme Corp",
                        Key = "acme",
                        AuthMode = AuthMode.Local,
                        IsActive = true
                    });
                }

                if (!await context.Tenant.AnyAsync(t => t.Id == contosoId))
                {
                    context.Tenant.Add(new Tenant
                    {
                        Id = contosoId,
                        Name = "Contoso",
                        Key = "contoso",
                        AuthMode = AuthMode.Local,
                        IsActive = true
                    });
                }

                // External IdP test tenant — update these values with a real IdP to test federation.
                // For example, Google: Authority = "https://accounts.google.com"
                // Auth0: Authority = "https://YOUR_DOMAIN.auth0.com"
                if (!await context.Tenant.AnyAsync(t => t.Id == fabrikamId))
                {
                    context.Tenant.Add(new Tenant
                    {
                        Id = fabrikamId,
                        Name = "Fabrikam (External)",
                        Key = "fabrikam",
                        AuthMode = AuthMode.ExternalOidc,
                        ExternalAuthority = "https://YOUR-IDP.example.com",
                        ExternalClientId = "YOUR_CLIENT_ID",
                        ExternalClientSecret = "YOUR_CLIENT_SECRET",
                        IsActive = true
                    });
                }

                await context.SaveChangesAsync();
            }

            static async Task SeedTestUsersAsync(IServiceProvider provider)
            {
                var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

                var testUsers = new[]
                {
                    new { Email = "acme@test.com", TenantId = Guid.Parse("A1000000-0000-0000-0000-000000000001") },
                    new { Email = "contoso@test.com", TenantId = Guid.Parse("A1000000-0000-0000-0000-000000000002") }
                };

                foreach (var testUser in testUsers)
                {
                    if (await userManager.FindByIdAsync(testUser.TenantId.ToString()) is null
                        && (await userManager.FindByEmailAsync(testUser.Email)) is null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = testUser.Email,
                            Email = testUser.Email,
                            TenantId = testUser.TenantId,
                            EmailConfirmed = true
                        };
                        await userManager.CreateAsync(user, "Test1234!");
                    }
                }
            }

            /// <summary>
            /// Dynamically registers an OIDC authentication scheme per external tenant.
            /// Runs AFTER seeding so the tenants exist in the DB.
            /// </summary>
            static async Task RegisterExternalSchemesAsync(IServiceProvider provider, AuthDbContext context)
            {
                var externalTenants = await context.Tenant
                    .Where(t => t.AuthMode == AuthMode.ExternalOidc && t.IsActive)
                    .ToListAsync();

                if (externalTenants.Count == 0)
                    return;

                var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
                var optionsCache = provider.GetRequiredService<IOptionsMonitorCache<OpenIdConnectOptions>>();
                var postConfigures = provider.GetServices<IPostConfigureOptions<OpenIdConnectOptions>>();

                foreach (var tenant in externalTenants)
                {
                    var schemeName = $"oidc-{tenant.Key}";

                    // Skip if already registered (e.g. from a previous run without restart)
                    if (await schemeProvider.GetSchemeAsync(schemeName) is not null)
                        continue;

                    // Build and post-configure the options
                    var options = new OpenIdConnectOptions
                    {
                        SignInScheme = IdentityConstants.ExternalScheme,
                        Authority = tenant.ExternalAuthority,
                        ClientId = tenant.ExternalClientId,
                        ClientSecret = tenant.ExternalClientSecret,
                        ResponseType = OpenIdConnectResponseType.Code,
                        CallbackPath = $"/signin-oidc-{tenant.Key}",
                        MapInboundClaims = false
                    };

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");

                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";

                    // MapInboundClaims = false keeps clean JWT claim names throughout the app,
                    // but SignInManager.GetExternalLoginInfoAsync() requires ClaimTypes.NameIdentifier
                    // to build the provider key. Bridge the gap by copying "sub" after token validation.
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = ctx =>
                        {
                            var sub = ctx.Principal?.FindFirst("sub")?.Value;
                            if (sub is not null)
                            {
                                ((ClaimsIdentity)ctx.Principal!.Identity!).AddClaim(
                                    new Claim(ClaimTypes.NameIdentifier, sub));
                            }
                            return Task.CompletedTask;
                        }
                    };

                    // Run ASP.NET Core's post-configuration (sets up backchannel handler, etc.)
                    foreach (var pc in postConfigures)
                    {
                        pc.PostConfigure(schemeName, options);
                    }

                    optionsCache.TryAdd(schemeName, options);

                    schemeProvider.AddScheme(new AuthenticationScheme(
                        schemeName, tenant.Name, typeof(OpenIdConnectHandler)));
                }
            }
        }
    }
}
