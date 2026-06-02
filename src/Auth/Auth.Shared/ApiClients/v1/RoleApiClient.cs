using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.ApiClients;

namespace Dyvenix.Auth.Shared.ApiClients.v1;

public partial class RoleApiClient : ApiClientBase, IRoleService
{
	public RoleApiClient(HttpClient httpClient) : base(httpClient)
	{
	}

	public async Task<RoleDto?> GetById(string id)
		=> await GetAsync<RoleDto?>($"api/v1/role/GetById/{id}");

	public async Task<RoleDto?> GetByName(string name)
		=> await GetAsync<RoleDto?>($"api/v1/role/GetByName/{Uri.EscapeDataString(name)}");

	public async Task<IReadOnlyList<RoleDto>> GetAllByTenant(Guid tenantId)
		=> await GetAsync<IReadOnlyList<RoleDto>>($"api/v1/role/GetAllByTenant/{tenantId}");

	public async Task<string> Create(CreateRoleReq request)
		=> await PostAsync<string>("api/v1/role/Create", request);

	public async Task Update(UpdateRoleReq request)
		=> await PutAsync("api/v1/role/Update", request);

	public async Task Delete(string id)
		=> await DeleteAsync<bool>("api/v1/role/Delete", new { Id = id });
}
