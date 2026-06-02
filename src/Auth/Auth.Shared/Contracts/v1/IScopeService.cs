using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Requests.v1;

namespace Dyvenix.Auth.Shared.Contracts.v1;

public interface IScopeService
{
    Task<ScopeDto?> GetById(string id);
    Task<ScopeDto?> GetByName(string name);
    Task<IReadOnlyList<ScopeDto>> GetAll();
    Task<string> Create(CreateScopeReq request);
    Task Update(UpdateScopeReq request);
    Task Delete(string id);
}
