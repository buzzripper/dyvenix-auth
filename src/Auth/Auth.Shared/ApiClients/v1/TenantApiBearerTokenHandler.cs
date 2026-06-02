using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Dyvenix.Auth.Shared.ApiClients.v1;

public class TenantApiBearerTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization is null)
        {
            var authorizationHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

            if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorizationValue))
            {
                request.Headers.Authorization = authorizationValue;
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
