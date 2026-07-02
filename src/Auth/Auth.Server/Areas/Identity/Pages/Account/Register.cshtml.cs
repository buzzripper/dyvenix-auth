using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dyvenix.Auth.Server.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
	public IActionResult OnGet() => RedirectToPage("/Account/Login", new { area = "Identity" });
	public IActionResult OnPost() => RedirectToPage("/Account/Login", new { area = "Identity" });
}
