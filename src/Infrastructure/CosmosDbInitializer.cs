using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public class CosmosDbInitializer : IHostedService
{
    private readonly string _connectionString;
    private readonly string _databaseName;

    public CosmosDbInitializer()
    {
        _connectionString = Environment.GetEnvironmentVariable("CosmosDbConnection") ?? string.Empty;
        _databaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE") ?? "astro-db";
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_connectionString)) return;
        var client = new CosmosClient(_connectionString);
        var database = await client.CreateDatabaseIfNotExistsAsync(_databaseName, cancellationToken: cancellationToken);
        await database.Database.CreateContainerIfNotExistsAsync("access", "/session_id", cancellationToken: cancellationToken);
        await database.Database.CreateContainerIfNotExistsAsync("actions", "/session_id", cancellationToken: cancellationToken);
        await database.Database.CreateContainerIfNotExistsAsync("search_result", "/session_id", cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
