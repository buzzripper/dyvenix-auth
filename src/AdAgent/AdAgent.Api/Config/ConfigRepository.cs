using Dyvenix.AdAgent.Shared;
using System.Text.Json;

namespace Dyvenix.AdAgent.Api.Config;

public interface IConfigRepository
{
	AdAgentConfig GetConfig();
	void SaveConfig(AdAgentConfig config);
}

public class ConfigRepository : IConfigRepository
{
	private const string ConfigFileName = "adagent_config.json";
	private const string AuthConfigFileName = "adagent_auth.json";

	private static string _configFilePath;
	private static string _authConfigFilePath;
	private static AdAgentConfig? _config;

	static ConfigRepository()
	{
		var programDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
		var configDir = Path.Combine(programDataDir, AdAgentConstants.CompanyName, AdAgentConstants.ModuleTitle);
		_configFilePath = Path.Combine(configDir, ConfigFileName);
		_authConfigFilePath = Path.Combine(configDir, AuthConfigFileName);
	}

	public AdAgentConfig GetConfig()
	{
		if (_config == null)
		{
			_config = LoadConfig();
			if (_config == null)
				throw new Exception("Failed to load configuration");

			if (_config.AuthMode == AdAgentAuthMode.Ldap)
			{
				var authConfig = LoadAuthConfig();
				if (authConfig == null)
					throw new Exception($"AuthMode is set to {AdAgentAuthMode.Ldap}, but AuthConfig is missing.");

				_config.AuthConfig = authConfig;
			}
		}
		return _config;
	}

	private AdAgentConfig LoadConfig()
	{
		if (!File.Exists(_configFilePath))
			throw new FileNotFoundException($"Configuration file not found at path: {_configFilePath}");

		var json = File.ReadAllText(_configFilePath);

		var config = JsonSerializer.Deserialize<AdAgentConfig>(json) ?? throw new Exception("Failed to deserialize configuration");
		return ScrubConfig(config);
	}

	private AdAgentConfig ScrubConfig(AdAgentConfig config)
	{
		if (!string.IsNullOrEmpty(config.DcHost))
			config.DcHost = config.DcHost.Trim();
		if (!string.IsNullOrEmpty(config.Domain))
			config.Domain = config.Domain.Trim();
		if (!string.IsNullOrEmpty(config.BaseDn))
			config.BaseDn = config.BaseDn.Trim();
		return config;
	}

	private AdAgentAuthConfig LoadAuthConfig()
	{
		if (!File.Exists(_authConfigFilePath))
			throw new FileNotFoundException($"Authentication configuration file not found at path: {_authConfigFilePath}");
		var json = File.ReadAllText(_authConfigFilePath);
		var authConfig = JsonSerializer.Deserialize<AdAgentAuthConfig>(json) ?? throw new Exception("Failed to deserialize authentication configuration");
		return ScrubAuthConfig(authConfig);
	}

	private AdAgentAuthConfig ScrubAuthConfig(AdAgentAuthConfig config)
	{
		if (!string.IsNullOrEmpty(config.ServiceUsername))
			config.ServiceUsername = config.ServiceUsername.Trim();
		if (!string.IsNullOrEmpty(config.ServicePassword))
			config.ServicePassword = config.ServicePassword.Trim();
		return config;
	}

	public void SaveConfig(AdAgentConfig config)
	{
		var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

		var directory = Path.GetDirectoryName(_configFilePath);

		if (directory == null)
			throw new DirectoryNotFoundException($"Config file path not configured.");

		if (!Directory.Exists(directory))
			Directory.CreateDirectory(directory);

		File.WriteAllText(_configFilePath, json);

		_config = config; // Update the in-memory config
	}
}
