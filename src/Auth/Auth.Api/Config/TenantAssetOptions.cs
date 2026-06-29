namespace Dyvenix.Auth.Api.Config;

public class TenantAssetOptions
{
	public string Provider { get; set; } = "File";
	public FileTenantAssetOptions File { get; set; } = new();
	public AzureBlobTenantAssetOptions AzureBlob { get; set; } = new();
}

public class FileTenantAssetOptions
{
	public string BasePath { get; set; } = "";
}

public class AzureBlobTenantAssetOptions
{
	public string ConnectionString { get; set; } = "";
	public string ContainerName { get; set; } = "tenant-assets";
}
