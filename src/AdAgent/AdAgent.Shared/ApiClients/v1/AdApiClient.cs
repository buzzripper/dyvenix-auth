using Dyvenix.AdAgent.Shared.Contracts.v1;
using Dyvenix.AdAgent.Shared.DTOs;
using Dyvenix.AdAgent.Shared.Requests.v1;
using Dyvenix.Core.ApiClients;

namespace Dyvenix.AdAgent.Shared.ApiClients.v1;

public partial class AdApiClient : ApiClientBase, IAdService
{
	public AdApiClient(HttpClient httpClient) : base(httpClient)
	{
	}

	public async Task<AdAuthResult> AuthenticateUser(string userUpnOrDomainUser, string password, CancellationToken ct = default)
	{
		var request = new AuthenticateUserReq
		{
			UserUpnOrDomainUser = userUpnOrDomainUser,
			Password = password
		};
		return await PostAsync<AdAuthResult>($"api/v1/ad/AuthenticateUser", request);
	}
}

