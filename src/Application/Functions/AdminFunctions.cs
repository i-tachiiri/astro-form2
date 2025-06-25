using System.Net;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Application.Functions;

[ExcludeFromCodeCoverage]
public class AdminFunctions
{
    private readonly ILogger _logger;
    private readonly string _connectionString;
    private readonly string _databaseName;

    public AdminFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdminFunctions>();
        _connectionString = Environment.GetEnvironmentVariable("CosmosDbConnection") ?? string.Empty;
        _databaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE") ?? "astro-db";
    }

    [Function("InitializeDatabase")]
    public async Task<HttpResponseData> InitializeDatabase([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test-data/initialize")] HttpRequestData req)
    {
        try
        {
            var client = new CosmosClient(_connectionString);
            var database = await client.CreateDatabaseIfNotExistsAsync(_databaseName);
            await database.Database.CreateContainerIfNotExistsAsync("access", "/session_id");
            await database.Database.CreateContainerIfNotExistsAsync("actions", "/session_id");
            await database.Database.CreateContainerIfNotExistsAsync("search_result", "/session_id");

            return req.CreateResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initialization failed");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("SeedTestData")]
    public async Task<HttpResponseData> SeedTestData([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "test-data/seed")] HttpRequestData req)
    {
        try
        {
            var environment =
                Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") ??
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                "Production";

            if (environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("SeedTestData is disabled in production environment.");
                return req.CreateResponse(HttpStatusCode.Forbidden);
            }

            var client = new CosmosClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            var accessContainer = database.GetContainer("access");
            var actionsContainer = database.GetContainer("actions");
            var searchContainer = database.GetContainer("search_result");

            await ClearContainerAsync(accessContainer);
            await ClearContainerAsync(actionsContainer);
            await ClearContainerAsync(searchContainer);

            string seedDir = Path.Combine(Environment.CurrentDirectory, "seed");
            if (Directory.Exists(seedDir))
            {
                await InsertItemsAsync(accessContainer, Path.Combine(seedDir, "access.json"));
                await InsertItemsAsync(actionsContainer, Path.Combine(seedDir, "actions.json"));
                await InsertItemsAsync(searchContainer, Path.Combine(seedDir, "search_result.json"));
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Seed failed");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    private static async Task InsertItemsAsync(Container container, string path)
    {
        if (!File.Exists(path)) return;
        using var stream = File.OpenRead(path);
        var items = await JsonSerializer.DeserializeAsync<JsonElement[]>(stream);
        if (items == null) return;
        foreach (var item in items)
        {
            await container.UpsertItemAsync(item);
        }
    }


    private static async Task ClearContainerAsync(Container container)
    {
        var iterator = container.GetItemQueryIterator<dynamic>("SELECT c.id, c.session_id FROM c");
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            foreach (var item in response)
            {
                try
                {
                    await container.DeleteItemAsync<dynamic>(
                        (string)item.id,
                        new PartitionKey((string)item.session_id));
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    // 存在しないだけなら無視
                }
            }
        }
    }
}
