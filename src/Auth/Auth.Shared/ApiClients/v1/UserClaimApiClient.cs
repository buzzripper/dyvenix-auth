using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.ApiClients;

namespace Dyvenix.Auth.Shared.ApiClients.v1;

public partial class UserClaimApiClient : ApiClientBase, IUserClaimService
{
	public UserClaimApiClient(HttpClient httpClient) : base(httpClient)
	{
	}

	public async Task<UserClaimDto?> GetById(int id)
		=> await GetAsync<UserClaimDto?>($"api/v1/userclaim/GetById/{id}");

	public async Task<IReadOnlyList<UserClaimDto>> GetAllByUser(string userId)
		=> await GetAsync<IReadOnlyList<UserClaimDto>>($"api/v1/userclaim/GetAllByUser/{userId}");

	public async Task Create(CreateUserClaimReq request)
		=> await PostAsync("api/v1/userclaim/Create", request);

	public async Task Update(UpdateUserClaimReq request)
		=> await PutAsync("api/v1/userclaim/Update", request);

	public async Task Delete(int id)
		=> await DeleteAsync<bool>("api/v1/userclaim/Delete", new { Id = id });
}
