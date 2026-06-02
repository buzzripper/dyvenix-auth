//using Dyvenix.AdAgent.Api.Config;
//using Dyvenix.AdAgent.Api.Endpoints.v1;
//using Scalar.AspNetCore;
//using Cv1 = Dyvenix.AdAgent.Shared.Contracts.v1;
//using Sv1 = Dyvenix.AdAgent.Api.Services.v1;

//namespace Dyvenix.AdAgent.Api.Extensions;

//public static partial class AdAgentApiServiceCollExt
//{
//	public static IServiceCollection AddAdAgentApiServices(this IServiceCollection services, AdAgentConfig adAgentConfig)
//	{
//		// Add system-level services
//		services.AddCurrentUserServices();
//		services.AddOpenApi();
//		services.AddHealthChecks()
//			.AddCheck<HealthService>("AD Agent Service Health");

//		// Register services
//		services.AddScoped<IConfigRepository, ConfigRepository>();
//		services.AddSingleton(adAgentConfig);
//		services.AddScoped<Cv1.IAdService, Sv1.AdService>();
//		services.AddScoped<ApiExceptionFilter<Sv1.AdService>>();

//		return services;
//	}

//	/// <summary> 
//	/// Maps endpoints
//	/// </summary>
//	public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
//	{
//		app.MapAdEndpoints();
//		return app;
//	}

//	/// <summary> 
//	/// Maps OpenAPI and Scalar API documentation endpoints for Auth API.
//	/// Call this in development or when you want to expose API documentation.
//	/// </summary>
//	public static IEndpointRouteBuilder MapAdAgentApiDocumentation(this IEndpointRouteBuilder app)
//	{
//		app.MapOpenApi();
//		app.MapScalarApiReference(options =>
//		{
//			options
//				.WithTitle("Auth API")
//				.WithTheme(ScalarTheme.DeepSpace);
//		});

//		return app;
//	}
//}
