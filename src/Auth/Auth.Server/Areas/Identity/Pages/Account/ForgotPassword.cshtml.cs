// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Dyvenix.Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dyvenix.Auth.Server.Areas.Identity.Pages.Account
{
	public class ForgotPasswordModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<ForgotPasswordModel> _logger;

		public ForgotPasswordModel(UserManager<ApplicationUser> userManager, ILogger<ForgotPasswordModel> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			public string Email { get; set; }
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
				return Page();

			var user = await _userManager.FindByEmailAsync(Input.Email);

			// Do not reveal whether the user exists
			if (user == null)
				return RedirectToPage("./ForgotPasswordConfirmation");

			var code = await _userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			var callbackUrl = Url.Page(
				"/Account/ResetPassword",
				pageHandler: null,
				values: new { area = "Identity", code, email = Input.Email },
				protocol: Request.Scheme);

			// TODO: Replace with a real email sender. The reset link is logged for development purposes.
			_logger.LogInformation("Password reset link for {Email}: {Url}", Input.Email, callbackUrl);

			return RedirectToPage("./ForgotPasswordConfirmation");
		}
	}
}
