using Dyvenix.AdAgent.Shared.DTOs;

namespace Dyvenix.AdAgent.Shared.Contracts.v1;

public interface IAdService
{
	Task<AdAuthResult> AuthenticateUser(string userUpnOrDomainUser, string password, CancellationToken ct = default);
}
