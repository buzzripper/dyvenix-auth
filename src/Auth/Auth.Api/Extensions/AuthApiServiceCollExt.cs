using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dyvenix.Auth.Api.Config;
using Dyvenix.Auth.Api.Endpoints;
using Dyvenix.Auth.Api.Repositories;
using Dyvenix.Auth.Api.Services;
using Dyvenix.Auth.Api.Services.v1;
using Dyvenix.Auth.Endpoints.v1;
using Dyvenix.Auth.Shared.Contracts;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Common.Api.Extensions;
using Dyvenix.Common.Api.Filters;
using Scalar.AspNetCore;
using sv1 = Dyvenix.Auth.Api.Services.v1;
using cv1 = Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Api.Endpoints.v1;

namespace Dyvenix.Auth.Api.Extensions;

public static partial class AuthApiServiceCollExt
{
    /// <summary>
    /// Registers Auth API services.
    /// Call this when hosting Auth services (standalone or in-process).
    /// </summary>
    public static IServiceCollection AddAuthApiServices(this IServiceCollection services, bool isInProcess)
    {
        services.AddCurrentUserServices();

        // Register business logic services
        services.AddScoped<BrandImgService>();
        services.AddScoped<IOidcAppService, OidcAppService>();
        services.AddScoped<ApiExceptionFilter<OidcAppService>>();

        if (!isInProcess)
        {
            // Add OpenAPI support
            services.AddOpenApi();
        }

        // Register services

		// TenantService
		services.AddScoped<cv1.ITenantService, sv1.TenantService>();
		services.AddScoped<ApiExceptionFilter<sv1.TenantService>>();

		// UserService
		services.AddScoped<cv1.IUserService, sv1.UserService>();
		services.AddScoped<ApiExceptionFilter<sv1.UserService>>();

		// UserClaimService
		services.AddScoped<cv1.IUserClaimService, sv1.UserClaimService>();
		services.AddScoped<ApiExceptionFilter<sv1.UserClaimService>>();

		// RoleService
		services.AddScoped<cv1.IRoleService, sv1.RoleService>();
		services.AddScoped<ApiExceptionFilter<sv1.RoleService>>();

		// RoleClaimService
		services.AddScoped<cv1.IRoleClaimService, sv1.RoleClaimService>();
		services.AddScoped<ApiExceptionFilter<sv1.RoleClaimService>>();

		// AppRegistrationService
		services.AddScoped<cv1.IAppRegistrationService, sv1.AppRegistrationService>();
		services.AddScoped<ApiExceptionFilter<sv1.AppRegistrationService>>();

		// ScopeService
		services.AddScoped<cv1.IScopeService, sv1.ScopeService>();
		services.AddScoped<ApiExceptionFilter<sv1.ScopeService>>();

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
        
        apiGroup.MapBrandImgEndpoints();
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
    /// Registers the brand image repository based on configuration.
    /// Call after AddAuthApiServices.
    /// </summary>
    public static IServiceCollection AddBrandImgRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BrandImgOptions>(configuration.GetSection("BrandImg"));

        var provider = configuration.GetValue<string>("BrandImg:Provider") ?? "File";
        if (provider.Equals("AzureBlob", StringComparison.OrdinalIgnoreCase))
            services.AddScoped<IBrandImgRepository, AzureBlobBrandImgRepository>();
        else
            services.AddScoped<IBrandImgRepository, FileBrandImgRepository>();

        return services;
    }
}
