using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dyvenix.Auth.Shared.DTOs;
using Dyvenix.Auth.Shared.Contracts.v1;
using Dyvenix.Auth.Shared.Requests.v1;

namespace Dyvenix.Auth.Shared.Contracts.v1;

public interface ITenantService
{
	Task<TenantDto?> GetById(Guid id);
	Task<TenantDto?> GetBySlug(string slug);
	Task<IReadOnlyList<TenantDto>> GetAll();
	Task<Guid> Create(CreateTenantReq request);
	Task Update(UpdateTenantReq request);
	Task Delete(Guid id);
}
