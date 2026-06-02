
using Dyvenix.Auth.Data.Context;

namespace Dyvenix.Auth.Server.Services
{
	public interface IClientRouter
	{
		Task<HttpClient> GetHttpClient(string clientKey);
	}

	public class ClientRouter : IClientRouter
	{
		private readonly Dictionary<string, string> _cache;
		private DateTime _expirationTimeUtc;
		private readonly AuthDbContext _dbContext;

		public ClientRouter(AuthDbContext dbContext)
		{
			_cache = new Dictionary<string, string>();
			_expirationTimeUtc = DateTime.UtcNow.AddDays(-1);
			_dbContext = dbContext;
		}

		public async Task<HttpClient> GetHttpClient(string clientKey)
		{
			if (_expirationTimeUtc.CompareTo(DateTime.UtcNow) < 0)
				await this.RefreshCache();

			if (!_cache.TryGetValue(clientKey, out var baseUrl))
				throw new Exception($"Client with key '{clientKey}' not found.");

			return new HttpClient { BaseAddress = new Uri(baseUrl) };
		}

		private async Task RefreshCache()
		{
			_cache.Clear();

			// TODO: implement this method to load client routes from the database. 
			// We'll leave it empty for now, it won't be needed until AD auth mode is
			// implemented and we need to route to the tenant's domain controller for authentication.
			/*
			var clientRouteDtos = await _clientService.GetAllClientRoutes();
			foreach (var clientRouteDto in clientRouteDtos)
				_cache.Add(clientRouteDto.Key, clientRouteDto.BaseUrl);
			*/

			_expirationTimeUtc = DateTime.UtcNow.AddMinutes(5);
		}
	}
}
