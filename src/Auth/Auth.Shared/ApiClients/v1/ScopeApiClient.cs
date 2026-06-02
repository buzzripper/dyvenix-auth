using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.ApiClients;

namespace Dyvenix.Auth.Shared.ApiClients.v1;

public partial class ScopeApiClient : ApiClientBase, IScopeService
{
	public ScopeApiClient(HttpClient httpClient) : base(httpClient)
	{
	}

	public async Task<ScopeDto?> GetById(string id)
		=> await GetAsync<ScopeDto?>($"api/v1/scope/GetById/{id}");

	public async Task<ScopeDto?> GetByName(string name)
		=> await GetAsync<ScopeDto?>($"api/v1/scope/GetByName/{Uri.EscapeDataString(name)}");

	public async Task<IReadOnlyList<ScopeDto>> GetAll()
		=> await GetAsync<IReadOnlyList<ScopeDto>>("api/v1/scope/GetAll");

	public async Task<string> Create(CreateScopeReq request)
		=> await PostAsync<string>("api/v1/scope/Create", request);

	public async Task Update(UpdateScopeReq request)
		=> await PutAsync("api/v1/scope/Update", request);

	public async Task Delete(string id)
		=> await DeleteAsync<bool>("api/v1/scope/Delete", new { Id = id });
}
