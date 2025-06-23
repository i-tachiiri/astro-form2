using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Domain.Repositories;
using Infrastructure;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        var sourcePath = Path.Combine("..", "..", "#config", "appsettings.Development.json");
        var destPath = Path.Combine(Environment.CurrentDirectory, "appsettings.Development.json");
        File.Copy(sourcePath, destPath, true);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Failed to copy configuration file: {ex.Message}");
    }
}

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddHttpClient();

var keyVaultUri = Environment.GetEnvironmentVariable("KEY_VAULT_URI");
KeyVaultService? keyVaultService = null;
if (!string.IsNullOrEmpty(keyVaultUri))
{
    keyVaultService = new KeyVaultService(keyVaultUri);
    builder.Services.AddSingleton(keyVaultService);
}

string cosmosConnection = Environment.GetEnvironmentVariable("COSMOS_CONNECTION") ?? string.Empty;
if (string.IsNullOrEmpty(cosmosConnection) && keyVaultService != null)
{
    cosmosConnection = keyVaultService.GetSecret("COSMOS_CONNECTION") ?? string.Empty;
}
string databaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE") ?? "astro-db";

builder.Services.AddSingleton<ILogRepository>(_ => new CosmosLogRepository(cosmosConnection, databaseName));

builder.Services.AddSingleton(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var logRepo = sp.GetRequiredService<ILogRepository>();
    var logger = sp.GetRequiredService<ILogger<Application.Services.BirthplaceSearchService>>();
    var kv = sp.GetService<KeyVaultService>();
    return new Application.Services.BirthplaceSearchService(httpClientFactory.CreateClient(), logRepo, logger, kv);
});

builder.Build().Run();
