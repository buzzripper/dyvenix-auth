using Dyvenix.Core.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Dyvenix.AdAgent.Shared.Extensions;

public static partial class AdAgentSharedServiceCollExt
{
	// Declaration of partial method for code-generated services
	static partial void AddGeneratedServices(IServiceCollection services);

	public static IServiceCollection AddAdAgentSharedServices(this IServiceCollection services, ApiClientConfig apiClientConfig, bool inProcess)
	{
		if (!inProcess)
		{
			string? baseUrl = apiClientConfig.BaseUrl;
			if (string.IsNullOrEmpty(apiClientConfig.BaseUrl))
			{
				throw new InvalidOperationException(
					"BaseUrl is missing from Auth configuration. It is required when InProcess is false");
			}

			// Add code-generated services
			AddGeneratedServices(services);
		}

		return services;
	}
}
