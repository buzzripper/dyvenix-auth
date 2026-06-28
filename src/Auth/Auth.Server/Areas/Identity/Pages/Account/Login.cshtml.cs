// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Dyvenix.AdAgent.Shared.ApiClients.v1;
using Dyvenix.AdAgent.Shared.DTOs;
using Dyvenix.Auth.Data;
using Dyvenix.Auth.Server.Services;
using Dyvenix.Auth.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Dyvenix.Auth.Server.Areas.Identity.Pages.Account
{
	public class LoginModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ITenantContext _tenantContext;
		private readonly ILogger<LoginModel> _logger;
		private readonly IClientRouter _clientRouter;

		public LoginModel(SignInManager<ApplicationUser> signInManager,
			ITenantContext tenantContext,
			ILogger<LoginModel> logger,
			IClientRouter clientRouter)
		{
			_signInManager = signInManager;
			_tenantContext = tenantContext;
			_logger = logger;
			_clientRouter = clientRouter;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string ReturnUrl { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string ErrorMessage { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[EmailAddress]
			public string Email { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}

			returnUrl ??= Url.Content("~/");

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			var tenant = _tenantContext.Tenant;

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
				if (user == null)
				{
					_logger.LogWarning("Login failed: no user found for {Email} in current tenant scope.", Input.Email);
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Page();
				}

				if (tenant.AuthMode == AuthMode.AD)
				{
					var httpClient = await _clientRouter.GetHttpClient(tenant.Key);
					var adApiClient = new AdApiClient(httpClient);
					var adAuthResult = await adApiClient.AuthenticateUser(Input.Email, Input.Password);
					if (adAuthResult.Status == AdAuthStatus.Success)
					{
						// AD authentication succeeded, sign in the user locally
						await _signInManager.SignInAsync(user, Input.RememberMe);
						_logger.LogInformation("User logged in with AD authentication.");
						return LocalRedirect(returnUrl);
					}
					else
					{
						_logger.LogWarning("AD authentication failed for {Email}.", Input.Email);
						ModelState.AddModelError(string.Empty, "Invalid login attempt.");
						return Page();
					}
				}
				else
				{
					// This doesn't count login failures towards account lockout
					// To enable password failures to trigger account lockout, set lockoutOnFailure: true
					var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: false);
					if (_logger.IsEnabled(LogLevel.Information))
					{
						_logger.LogInformation("PasswordSignIn for {Email}: Succeeded={Succeeded}, IsLockedOut={Locked}, RequiresTwoFactor={TwoFactor}, IsNotAllowed={NotAllowed}",
							Input.Email, result.Succeeded, result.IsLockedOut, result.RequiresTwoFactor, result.IsNotAllowed);
					}
					if (result.Succeeded)
					{
						_logger.LogInformation("User logged in.");
						return LocalRedirect(returnUrl);
					}
					if (result.RequiresTwoFactor)
					{
						throw new ApplicationException("Two factor authentication is not supported in this application. Please contact your administrator.");
					}
					if (result.IsLockedOut)
					{
						_logger.LogWarning("User account locked out.");
						return RedirectToPage("./Lockout");
					}
					else
					{
						ModelState.AddModelError(string.Empty, "Invalid login attempt.");
						return Page();
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
