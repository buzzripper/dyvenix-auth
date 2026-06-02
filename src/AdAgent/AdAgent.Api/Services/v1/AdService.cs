using Dyvenix.AdAgent.Api.Config;
using Dyvenix.AdAgent.Shared.Contracts.v1;
using Dyvenix.AdAgent.Shared.DTOs;
using System.DirectoryServices.Protocols;
using System.Net;

namespace Dyvenix.AdAgent.Api.Services.v1;

public class AdService(AdAgentConfig _config) : IAdService
{
	public async Task<AdAuthResult> AuthenticateUser(string userUpnOrDomainUser, string password, CancellationToken ct = default)
	{
		// First check the credentials
		try
		{
			if (!await ValidateUserCredentials(userUpnOrDomainUser, password, ct))
				return new AdAuthResult(AdAuthStatus.InvalidCredentials, "Invalid username or password");
		}
		catch (LdapException ex)
		{
			return MapLdapException(ex);
		}
		catch (Exception ex)
		{
			return new AdAuthResult(AdAuthStatus.UnknownError, ex.Message);
		}

		// Login succeeded, now get the unique identifier for the user - objectGuid
		try
		{
			// Don't supply svc credentials if we're using Kerberos auth - it should use the machine account, which hopefully has permissions to read user info.
			// If we're using LDAP auth, then we need to use the service account credentials to read user info.
			NetworkCredential? serviceCredentials = null;
			if (_config.AuthMode == AdAgentAuthMode.Ldap)
			{
				if (_config.AuthConfig == null)
					return new AdAuthResult(AdAuthStatus.InternalError, $"AuthMode is set to {AdAgentAuthMode.Ldap}, but the AuthConfig is missing.");
				serviceCredentials = new NetworkCredential(_config.AuthConfig.ServiceUsername, _config.AuthConfig.ServicePassword, _config.Domain);
			}

			var objectGuid = await GetUserObjectGuidAsync(userUpnOrDomainUser, serviceCredentials);
			if (objectGuid == null)
				return new AdAuthResult(AdAuthStatus.UserNotFound, "User not found");

			// Success!
			return new AdAuthResult(AdAuthStatus.Success, "Success", objectGuid);
		}
		catch (LdapException ex)
		{
			return MapLdapException(ex);
		}
		catch (Exception ex)
		{
			return new AdAuthResult(AdAuthStatus.UnknownError, $"Credentials validated successfully, but error attempting to find user in directory: {ex.Message}");
		}
	}

	private async Task<bool> ValidateUserCredentials(string userPrincipalName, string password, CancellationToken ct = default)
	{
		if (string.IsNullOrWhiteSpace(userPrincipalName))
			throw new ArgumentException("Email/username required", nameof(userPrincipalName));

		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Password required", nameof(password));

		return await Task.Run(() =>
		{
			var id = new LdapDirectoryIdentifier(_config.DcHost, _config.LdapPort);
			using var conn = new LdapConnection(id) { AuthType = AuthType.Negotiate };
			conn.SessionOptions.ProtocolVersion = 3;

			conn.Bind(new NetworkCredential(userPrincipalName, password));
			return true;
		}, ct);
	}

	private AdAuthResult MapLdapException(LdapException ex)
	{
		// Most AD failures return LDAP error 49 with sub-error codes in the message.
		if (ex.ErrorCode == 49)
		{
			if (ex.ServerErrorMessage?.Contains("775") == true)
				return new AdAuthResult(AdAuthStatus.AccountLocked, "Account locked");

			if (ex.ServerErrorMessage?.Contains("532") == true)
				return new AdAuthResult(AdAuthStatus.PasswordExpired, "Password expired");

			if (ex.ServerErrorMessage?.Contains("525") == true)
				return new AdAuthResult(AdAuthStatus.UserNotFound, "User not found");

			return new AdAuthResult(AdAuthStatus.InvalidCredentials, "Invalid username or password");
		}

		if (ex.ErrorCode == 81) // LDAP_SERVER_DOWN
			return new AdAuthResult(AdAuthStatus.DomainUnavailable, "Domain controller unavailable");

		return new AdAuthResult(AdAuthStatus.UnknownError, ex.Message);
	}

	public async Task<Guid?> GetUserObjectGuidAsync(string userPrincipalName, NetworkCredential? serviceCred, CancellationToken ct = default)
	{
		return await Task.Run(() =>
		{
			var id = new LdapDirectoryIdentifier(_config.DcHost, _config.LdapPort);
			using var conn = new LdapConnection(id) { AuthType = AuthType.Negotiate };
			conn.SessionOptions.ProtocolVersion = 3;

			if (serviceCred != null)
				conn.Bind(serviceCred);
			else
				conn.Bind(); // Use machine account credentials

			var filter = $"(userPrincipalName={EscapeLdapFilterValue(userPrincipalName)})";

			var req = new SearchRequest(
				_config.BaseDn,
				filter,
				SearchScope.Subtree,
				"objectGuid");

			var resp = (SearchResponse)conn.SendRequest(req);

			if (resp.Entries.Count == 0) return (Guid?)null;

			var bytes = (byte[])resp.Entries[0].Attributes["objectGUID"][0];
			return new Guid(bytes);
		}, ct);
	}

	private static string EscapeLdapFilterValue(string value)
		=> value.Replace(@"\", @"\5c")
				.Replace("*", @"\2a")
				.Replace("(", @"\28")
				.Replace(")", @"\29")
				.Replace("\0", @"\00");
}

