using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;
using Dyvenix.Core.ApiClients;

namespace Dyvenix.Auth.Shared.ApiClients.v1;

public partial class AppRegistrationApiClient : ApiClientBase, IAppRegistrationService
{
	public AppRegistrationApiClient(HttpClient httpClient) : base(httpClient)
	{
	}

	public async Task<AppRegistrationDto?> GetById(string id)
		=> await GetAsync<AppRegistrationDto?>($"api/v1/appregistration/GetById/{id}");

	public async Task<AppRegistrationDto?> GetByClientId(string clientId)
		=> await GetAsync<AppRegistrationDto?>($"api/v1/appregistration/GetByClientId/{Uri.EscapeDataString(clientId)}");

	public async Task<IReadOnlyList<AppRegistrationDto>> GetAll()
		=> await GetAsync<IReadOnlyList<AppRegistrationDto>>("api/v1/appregistration/GetAll");

	public async Task<string> Create(CreateAppRegistrationReq request)
		=> await PostAsync<string>("api/v1/appregistration/Create", request);

	public async Task Update(UpdateAppRegistrationReq request)
		=> await PutAsync("api/v1/appregistration/Update", request);

	public async Task Delete(string id)
		=> await DeleteAsync<bool>("api/v1/appregistration/Delete", new { Id = id });
}
