namespace Dyvenix.Auth.Api.Config;

public class BrandImgOptions
{
	public string Provider { get; set; } = "File";
	public FileBrandImgOptions File { get; set; } = new();
	public AzureBlobBrandImgOptions AzureBlob { get; set; } = new();
}

public class FileBrandImgOptions
{
	public string BasePath { get; set; } = "";
}

public class AzureBlobBrandImgOptions
{
	public string ConnectionString { get; set; } = "";
	public string ContainerName { get; set; } = "tenant-brand-images";
}
