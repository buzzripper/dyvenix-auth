using Dyvenix.Auth.Data;
using Dyvenix.Auth.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Dyvenix.Auth.Server;

/// <summary>
/// Middleware that resolves the current tenant from the acr_values query parameter (acr_values=tenant:key)
/// or from the return URL's acr_values during post-back flows. Sets the resolved tenant in ITenantContext
/// so it's available to EF Core query filters and downstream components.
/// </summary>
public class TenantResolutionMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context, AuthDbContext dbContext, ITenantContext tenantContext)
	{
		// Attempt to extract tenant key from acr_values or returnUrl query parameters
		var tenantKey = ExtractTenantKey(context);
		if (!string.IsNullOrEmpty(tenantKey))
		{
			// Query without the global filter to resolve the tenant itself
			var tenant = await dbContext.Tenant
				.AsNoTracking()
				.FirstOrDefaultAsync(t => t.Key == tenantKey && t.IsActive);

			if (tenant != null)
			{
				((TenantContext)tenantContext).Set(tenant);
			}
		}
		else
		{

		}
		await next(context);
	}

	private static string? ExtractTenantKey(HttpContext context)
	{
		// Check acr_values in query string (authorize endpoint)
		var acrValues = context.Request.Query["acr_values"].FirstOrDefault();

		// Check acr_values in form body (token endpoint POST)
		if (string.IsNullOrEmpty(acrValues) && context.Request.HasFormContentType)
		{
			acrValues = context.Request.Form["acr_values"].FirstOrDefault();
		}

		// Check returnUrl query param (login page redirects carry the original authorize URL)
		if (string.IsNullOrEmpty(acrValues))
		{
			var returnUrl = context.Request.Query["ReturnUrl"].FirstOrDefault()
				?? context.Request.Query["returnUrl"].FirstOrDefault();

			if (!string.IsNullOrEmpty(returnUrl) && Uri.TryCreate(returnUrl, UriKind.RelativeOrAbsolute, out var uri))
			{
				var queryString = uri.IsAbsoluteUri ? uri.Query : returnUrl.Contains('?') ? returnUrl[returnUrl.IndexOf('?')..] : null;
				if (queryString != null)
				{
					var qs = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(queryString);
					acrValues = qs.TryGetValue("acr_values", out var val) ? val.FirstOrDefault() : null;
				}
			}
		}

		if (string.IsNullOrEmpty(acrValues))
			return null;

		// Parse "tenant:key" from acr_values (may contain multiple space-separated values)
		foreach (var part in acrValues.Split(' ', StringSplitOptions.RemoveEmptyEntries))
		{
			if (part.StartsWith("tenant:", StringComparison.OrdinalIgnoreCase))
			{
				return part["tenant:".Length..];
			}
		}

		return null;
	}
}
