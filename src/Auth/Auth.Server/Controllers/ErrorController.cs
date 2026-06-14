/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using Dyvenix.Auth.Server.ViewModels.Shared;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Dyvenix.Auth.Server.Controllers;

public class ErrorController : Controller
{
	[Route("error")]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		// If the error was not caused by an invalid
		// OIDC request, display a generic error page.
		var response = HttpContext.GetOpenIddictServerResponse();
		if (response == null)
		{
			return View(new ErrorViewModel());
		}

		return View(new ErrorViewModel
		{
			Error = response.Error ?? string.Empty,
			ErrorDescription = response.ErrorDescription ?? string.Empty
		});
	}
}
