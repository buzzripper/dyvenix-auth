using Dyvenix.AdAgent.Api.Config;
using Dyvenix.AdAgent.Api.Endpoints.v1;
using Dyvenix.Auth.Api.Extensions;
using Dyvenix.Core.Api.Extensions.BuilderExtensions;
using Dyvenix.Core.Api.Extensions.SvcCollExtensions;
using Dyvenix.Core.Api.Extensions.WebAppExtensions;
using Dyvenix.Core.Api.Handlers;
using Scalar.AspNetCore;
using Cv1 = Dyvenix.AdAgent.Shared.Contracts.v1;
using Sv1 = Dyvenix.AdAgent.Api.Services.v1;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Host.UseWindowsService();
builder.ConfigureOpenTelemetry();

// Add .NET services
services.AddServiceDiscovery();
services.AddOpenApi();
services.AddExceptionHandler<ApiExceptionHandler>();

// Add Core services
services.AddDefaultHealthChecks();
services.AddStandardApiVersioning();
services.AddPermissionAuthorization();
if (builder.Environment.IsEnvironment("Testing"))
	services.AddTestJwtAuthentication();
else
	services.AddJwtBearerAuthentication(builder.Configuration);

services.AddAuthApiServices();

// Add AdAgent.API services
var adAgentConfig = new ConfigRepository().GetConfig();
services.AddScoped<IConfigRepository, ConfigRepository>();
services.AddSingleton(adAgentConfig);
services.AddScoped<Cv1.IAdService, Sv1.AdService>();
services.AddCurrentUserServices();


//----------------------------------------------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Common middleware
app.MapHealthEndpoints();

// Register permissions for this service
//app.Services.GetRequiredService<PermissionRegistry>()
//	.Register(AppPermissions.Hierarchy);

app.MapAdEndpoints();

// Enable API documentation in development
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference(options =>
	{
		options
			.WithTitle("Auth API")
			.WithTheme(ScalarTheme.DeepSpace);
	});
}

app.Run();
