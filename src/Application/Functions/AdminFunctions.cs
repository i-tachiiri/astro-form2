using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Infrastructure;
using System.Text.Json;
using System.IO;

namespace Application.Functions;

public class AdminFunctions
{
    private readonly ILogger _logger;

    public AdminFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AdminFunctions>();
    }

    [Function("AdminInitialize")]
    public HttpResponseData Initialize(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "admin/initialize")] HttpRequestData req)
    {
        try
        {
            var conn = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING") ?? string.Empty;
            var db = Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME") ?? "logs";
            _ = new CosmosLogRepository(conn, db);
            return req.CreateResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "initialize failed");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("AdminSeed")]
    public async Task<HttpResponseData> Seed(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "admin/seed")] HttpRequestData req)
    {
        try
        {
            var conn = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING") ?? string.Empty;
            var db = Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME") ?? "logs";
            var repo = new CosmosLogRepository(conn, db);

            var access = JsonSerializer.Deserialize<List<AccessLog>>(await File.ReadAllTextAsync(Path.Combine("seed", "access.json"))) ?? new();
            foreach (var a in access) await repo.AddAccessLogAsync(a);

            var actions = JsonSerializer.Deserialize<List<ActionLog>>(await File.ReadAllTextAsync(Path.Combine("seed", "actions.json"))) ?? new();
            foreach (var a in actions) await repo.AddActionLogAsync(a);

            var results = JsonSerializer.Deserialize<List<SearchResultLog>>(await File.ReadAllTextAsync(Path.Combine("seed", "search_result.json"))) ?? new();
            foreach (var r in results) await repo.AddSearchResultLogAsync(r);

            return req.CreateResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "seed failed");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
