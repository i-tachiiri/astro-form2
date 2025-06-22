using System.Net;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Application.Functions;

public class AdminFunctions
{
    private readonly ILogger _logger;
    private readonly string _connectionString;
    private readonly string _databaseName;

    public AdminFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdminFunctions>();
        _connectionString = Environment.GetEnvironmentVariable("COSMOS_CONNECTION") ?? string.Empty;
        _databaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE") ?? "astro-db";
    }

    [Function("InitializeDatabase")]
    public async Task<HttpResponseData> InitializeDatabase([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "admin/initialize")] HttpRequestData req)
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
    public async Task<HttpResponseData> SeedTestData([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "admin/seed")] HttpRequestData req)
    {
        try
        {
            var client = new CosmosClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            var accessContainer = database.GetContainer("access");
            var actionsContainer = database.GetContainer("actions");
            var searchContainer = database.GetContainer("search_result");

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
}
