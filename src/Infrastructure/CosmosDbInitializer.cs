using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.Json;

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
        var dbResponse = await client.CreateDatabaseIfNotExistsAsync(
            _databaseName,
            throughput: 1000,
            cancellationToken: cancellationToken);
        var database = dbResponse.Database;

        await database.CreateContainerIfNotExistsAsync("access", "/session_id", cancellationToken: cancellationToken);
        await database.CreateContainerIfNotExistsAsync("actions", "/session_id", cancellationToken: cancellationToken);
        await database.CreateContainerIfNotExistsAsync("search_result", "/session_id", cancellationToken: cancellationToken);

        var environment =
            Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ??
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
            "Production";

        if (!environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
        {
            var db = client.GetDatabase(_databaseName);
            var access = db.GetContainer("access");
            var actions = db.GetContainer("actions");
            var search = db.GetContainer("search_result");

            var seedDir = Path.Combine(Environment.CurrentDirectory, "seed");
            await InsertItemsAsync(access, Path.Combine(seedDir, "access.json"));
            await InsertItemsAsync(actions, Path.Combine(seedDir, "actions.json"));
            await InsertItemsAsync(search, Path.Combine(seedDir, "search_result.json"));
        }
    }

    private static async Task InsertItemsAsync(Container container, string path)
    {
        if (!File.Exists(path)) return;

        var json = await File.ReadAllTextAsync(path);
        var items = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);
        if (items == null) return;

        foreach (var item in items)
        {
            var normalized = new Dictionary<string, object>();

            foreach (var kvp in item)
            {
                normalized[kvp.Key] = kvp.Value.ValueKind switch
                {
                    JsonValueKind.String => kvp.Value.GetString(),
                    JsonValueKind.Number => kvp.Value.TryGetInt64(out var l) ? l : kvp.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null!,
                    _ => kvp.Value.ToString() // fallback
                };
            }

            // パーティションキーがなければ追加
            if (!normalized.ContainsKey("session_id"))
                normalized["session_id"] = Guid.NewGuid().ToString();

            if (!normalized.ContainsKey("id"))
                normalized["id"] = Guid.NewGuid().ToString();

            await container.UpsertItemAsync(normalized);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
