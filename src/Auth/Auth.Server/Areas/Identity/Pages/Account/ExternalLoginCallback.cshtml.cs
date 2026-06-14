using Dyvenix.Auth.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Dyvenix.Auth.Server.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ExternalLoginCallbackModel(
	SignInManager<ApplicationUser> signInManager,
	UserManager<ApplicationUser> userManager,
	ITenantContext tenantContext,
	ILogger<ExternalLoginCallbackModel> logger) : PageModel
{
	public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
	{
		returnUrl ??= Url.Content("~/");

		logger.LogInformation("ExternalLoginCallback invoked. Tenant: {Slug} ({TenantId}), returnUrl length: {Len}",
			tenantContext.TenantSlug, tenantContext.TenantId, returnUrl?.Length);

		// Read the external identity set by the OIDC middleware (stored in Identity.External cookie).
		var info = await signInManager.GetExternalLoginInfoAsync();
		if (info is null)
		{
			// Diagnostic: try to read the external cookie directly to understand WHY it failed.
			var authResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
			logger.LogWarning(
				"GetExternalLoginInfoAsync returned null. External cookie present: {HasPrincipal}, " +
				"Properties items: {Items}, Claims: {Claims}",
				authResult?.Principal is not null,
				authResult?.Properties?.Items is not null
					? string.Join("; ", authResult.Properties.Items.Select(kv => $"{kv.Key}={kv.Value}"))
					: "(none)",
				authResult?.Principal is not null
					? string.Join("; ", authResult.Principal.Claims.Select(c => $"{c.Type}={c.Value}"))
					: "(none)");

			return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
		}

		logger.LogInformation("External info: Provider={Provider}, ProviderKey={Key}",
			info.LoginProvider, info.ProviderKey);

		// Try to sign in using an existing external login link.
		var signInResult = await signInManager.ExternalLoginSignInAsync(
			info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

		if (signInResult.Succeeded)
		{
			logger.LogInformation("User signed in via {Provider}.", info.LoginProvider);
			return LocalRedirect(returnUrl);
		}

		logger.LogInformation("ExternalLoginSignInAsync result: Succeeded={S}, IsLockedOut={L}, IsNotAllowed={N}",
			signInResult.Succeeded, signInResult.IsLockedOut, signInResult.IsNotAllowed);

		// No linked account — create a new user for this tenant.
		// Entra CIAM may use "preferred_username" or "emails" instead of "email".
		var email = info.Principal.FindFirstValue("email")
			?? info.Principal.FindFirstValue(ClaimTypes.Email)
			?? info.Principal.FindFirstValue("preferred_username")
			?? info.Principal.FindFirstValue("emails");
		var name = info.Principal.FindFirstValue("name")
			?? info.Principal.FindFirstValue(ClaimTypes.Name)
			?? email;

		if (string.IsNullOrEmpty(email))
		{
			logger.LogError("External provider did not return an email claim. Available claims: {Claims}",
				string.Join("; ", info.Principal.Claims.Select(c => $"{c.Type}={c.Value}")));
			return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
		}

		logger.LogInformation("Creating user {Email} for tenant {TenantId}", email, tenantContext.TenantId);

		var user = new ApplicationUser
		{
			UserName = email,
			Email = email,
			TenantId = tenantContext.TenantId,
			EmailConfirmed = true
		};

		var createResult = await userManager.CreateAsync(user);
		if (!createResult.Succeeded)
		{
			// If the only failure is a duplicate username, the user already exists — just link
			// the external login to the existing account and sign in.
			var duplicateUserName = createResult.Errors.All(e => e.Code == "DuplicateUserName");
			if (duplicateUserName)
			{
				var existingUser = await userManager.FindByEmailAsync(email);
				if (existingUser is not null)
				{
					var linkLoginResult = await userManager.AddLoginAsync(existingUser, info);
					if (linkLoginResult.Succeeded)
					{
						await signInManager.SignInAsync(existingUser, isPersistent: false);
						logger.LogInformation("Linked external login and signed in existing user {Email} via {Provider}.",
							email, info.LoginProvider);
						return LocalRedirect(returnUrl);
					}
					logger.LogError("Failed to link external login to existing user: {Errors}",
						string.Join(", ", linkLoginResult.Errors.Select(e => e.Description)));
				}
			}

			logger.LogError("Failed to create user: {Errors}",
				string.Join(", ", createResult.Errors.Select(e => e.Description)));
			return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
		}

		// Link the external login to the new user.
		var addLoginResult = await userManager.AddLoginAsync(user, info);
		if (!addLoginResult.Succeeded)
		{
			logger.LogError("Failed to link external login: {Errors}",
				string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
			return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
		}

		// Sign in the new user with the application cookie.
		await signInManager.SignInAsync(user, isPersistent: false);
		logger.LogInformation("Created and signed in user {Email} for tenant {Tenant} via {Provider}.",
			email, tenantContext.TenantSlug, info.LoginProvider);

		return LocalRedirect(returnUrl);
	}
}
