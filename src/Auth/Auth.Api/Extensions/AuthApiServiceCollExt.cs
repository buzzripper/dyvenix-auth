using Dyvenix.Auth.Api.Config;
using Dyvenix.Auth.Api.Endpoints;
using Dyvenix.Auth.Api.Endpoints.v1;
using Dyvenix.Auth.Api.Repositories;
using Dyvenix.Auth.Api.Services;
using Dyvenix.Auth.Api.Services.v1;
using Dyvenix.Auth.Shared.Contracts;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Core.Api.Handlers;
using Dyvenix.Core.Contracts;
using Dyvenix.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using cv1 = Dyvenix.Auth.Shared.Contracts.v1;
using sv1 = Dyvenix.Auth.Api.Services.v1;

namespace Dyvenix.Auth.Api.Extensions;

public static partial class AuthApiServiceCollExt
{
	/// <summary>
	/// Registers Auth API services.
	/// Call this when hosting Auth services (standalone or in-process).
	/// </summary>
	public static IServiceCollection AddAuthApiServices(this IServiceCollection services)
	{
		// HttpClient for calling other services
		services.AddHttpContextAccessor();
		services.ConfigureHttpClientDefaults(http =>
		{
			// Turn on resilience by default
			http.AddStandardResilienceHandler();

			// Turn on service discovery by default
			http.AddServiceDiscovery();
		});

		// Register business logic services
		services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
		services.AddScoped<ITenantAssetService, TenantAssetService>();

		services.AddExceptionHandler<ApiExceptionHandler>();

		// APIs for OpenIdDict
		services.AddScoped<IOidcAppService, OidcAppService>();

		// Add OpenAPI support
		services.AddOpenApi();

		// Register v1 services
		services.AddScoped<cv1.ITenantService, sv1.TenantService>();
		services.AddScoped<cv1.IUserService, sv1.UserService>();
		services.AddScoped<cv1.IUserClaimService, sv1.UserClaimService>();
		services.AddScoped<cv1.IRoleService, sv1.RoleService>();
		services.AddScoped<cv1.IRoleClaimService, sv1.RoleClaimService>();
		services.AddScoped<cv1.IAppRegistrationService, sv1.AppRegistrationService>();
		services.AddScoped<cv1.IScopeService, sv1.ScopeService>();

		return services;
	}

	/// <summary> 
	/// Maps endpoints
	/// </summary>
	public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
	{
		var apiGroup = app.MapGroup("")
			.RequireAuthorization(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute
			{
				AuthenticationSchemes = "OpenIddict.Validation.AspNetCore"
			});

		apiGroup.MapTenantAssetEndpoints();
		apiGroup.MapOidcAppEndpoints();

		// Map API endpoints
		apiGroup.MapTenantEndpoints();
		apiGroup.MapUserEndpoints();
		apiGroup.MapUserClaimEndpoints();
		apiGroup.MapRoleEndpoints();
		apiGroup.MapRoleClaimEndpoints();
		apiGroup.MapAppRegistrationEndpoints();
		apiGroup.MapScopeEndpoints();

		return app;
	}

	/// <summary>
	/// Maps OpenAPI and Scalar API documentation endpoints for Auth API.
	/// Call this in development or when you want to expose API documentation.
	/// </summary>
	public static IEndpointRouteBuilder MapAuthApiDocumentation(this IEndpointRouteBuilder app)
	{
		app.MapOpenApi();
		app.MapScalarApiReference(options =>
		{
			options
				.WithTitle("Auth API")
				.WithTheme(ScalarTheme.DeepSpace);
		});

		return app;
	}

	/// <summary>
	/// Registers the tenant asset repository based on configuration.
	/// Call after AddAuthApiServices.
	/// </summary>
	public static IServiceCollection AddTenantAssetRepository(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<TenantAssetOptions>(configuration.GetSection("TenantAssets"));

		var provider = configuration.GetValue<string>("TenantAssets:Provider") ?? "File";
		if (provider.Equals("AzureBlob", StringComparison.OrdinalIgnoreCase))
			services.AddScoped<ITenantAssetRepository, AzureBlobTenantAssetRepository>();
		else
			services.AddScoped<ITenantAssetRepository, FileTenantAssetRepository>();

		return services;
	}
}
