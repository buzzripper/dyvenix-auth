using Dyvenix.Auth.Shared.ApiClients.v1;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Core.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Dyvenix.Auth.Shared.Extensions;

public static partial class AuthSharedServiceCollExt
{
	public static IServiceCollection AddAuthSharedServices(this IServiceCollection services, ApiClientConfig apiClientConfig)
	{
		if (string.IsNullOrEmpty(apiClientConfig.BaseUrl))
		{
			throw new InvalidOperationException(
				"BaseUrl is missing from Auth configuration. It is required when InProcess is false");
		}

		services.AddHttpContextAccessor();
		services.AddTransient<TenantApiBearerTokenHandler>();

		// TenantService
		services.AddHttpClient<ITenantService, TenantApiClient>(client =>
		{
			client.BaseAddress = new Uri(apiClientConfig.BaseUrl);
		})
		.AddHttpMessageHandler<TenantApiBearerTokenHandler>();

		return services;
	}
}
