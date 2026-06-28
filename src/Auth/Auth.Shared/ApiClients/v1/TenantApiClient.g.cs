using Dyvenix.Core.ApiClients;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;

namespace Dyvenix.Auth.Shared.ApiClients.v1;

public partial class TenantApiClient : ApiClientBase, ITenantService
{
	public TenantApiClient(HttpClient httpClient) : base(httpClient)
	{
	}

	public async Task<TenantDto?> GetById(Guid id)
	{
		return await GetAsync<TenantDto?>($"api/v1/tenant/GetById/{id}");
	}

	public async Task<TenantDto?> GetByKey(string key)
	{
		return await GetAsync<TenantDto?>($"api/v1/tenant/GetByKey/{Uri.EscapeDataString(key)}");
	}

	public async Task<IReadOnlyList<TenantDto>> GetAll()
	{
		return await GetAsync<IReadOnlyList<TenantDto>>("api/v1/tenant/GetAll");
	}

	public async Task<Guid> Create(CreateTenantReq request)
	{
		return await PostAsync<Guid>("api/v1/tenant/Create", request);
	}

	public async Task Update(UpdateTenantReq request)
	{
		await PutAsync("api/v1/tenant/Update", request);
	}

	public async Task Delete(Guid id)
	{
		await DeleteAsync("api/v1/tenant/Delete", id);
	}
}
