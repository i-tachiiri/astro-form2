using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Domain.Repositories;
using Infrastructure;

[assembly: ExcludeFromCodeCoverage]

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        var sourcePath = Path.Combine("..", "..", "#config", "local.settings.json");
        var destPath = Path.Combine(Environment.CurrentDirectory, "local.settings.json");
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

builder.Services.AddSingleton<ILogRepository>(_ =>
    new CosmosLogRepository(
        Environment.GetEnvironmentVariable("CosmosDbConnection") ?? string.Empty,
        Environment.GetEnvironmentVariable("COSMOS_DATABASE") ?? "astro-db"));

builder.Services.AddSingleton<Application.Services.BirthplaceSearchService>();

builder.Services.AddHostedService<CosmosDbInitializer>();

builder.Build().Run();
