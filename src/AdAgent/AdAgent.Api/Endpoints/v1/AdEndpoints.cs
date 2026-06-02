using Dyvenix.AdAgent.Shared.Contracts.v1;
using Dyvenix.AdAgent.Shared.DTOs;
using Dyvenix.AdAgent.Shared.Requests.v1;
using Dyvenix.Core.DTOs;

namespace Dyvenix.AdAgent.Api.Endpoints.v1;

public static class AdEndpoints
{
	public static IEndpointRouteBuilder MapAdEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("api/v1/ad")
			.WithTags("AdService");

		group.MapPost("AuthenticateUser", AuthenticateUser)
			.Produces<Guid>(StatusCodes.Status200OK);

		return app;
	}

	public static async Task<Result<AdAuthResult>> AuthenticateUser(IAdService adService, AuthenticateUserReq request)
	{
		var result = await adService.AuthenticateUser(request.UserUpnOrDomainUser, request.Password);
		return Result<AdAuthResult>.Ok(result);
	}

}
