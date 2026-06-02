using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.ApiClients;

namespace Dyvenix.Auth.Shared.ApiClients.v1;

public partial class RoleClaimApiClient : ApiClientBase, IRoleClaimService
{
	public RoleClaimApiClient(HttpClient httpClient) : base(httpClient)
	{
	}

	public async Task<RoleClaimDto?> GetById(int id)
		=> await GetAsync<RoleClaimDto?>($"api/v1/roleclaim/GetById/{id}");

	public async Task<IReadOnlyList<RoleClaimDto>> GetAllByRole(string roleId)
		=> await GetAsync<IReadOnlyList<RoleClaimDto>>($"api/v1/roleclaim/GetAllByRole/{roleId}");

	public async Task Create(CreateRoleClaimReq request)
		=> await PostAsync("api/v1/roleclaim/Create", request);

	public async Task Update(UpdateRoleClaimReq request)
		=> await PutAsync("api/v1/roleclaim/Update", request);

	public async Task Delete(int id)
		=> await DeleteAsync<bool>("api/v1/roleclaim/Delete", new { Id = id });
}
