using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;

namespace Infrastructure;

public class CosmosLogRepository : ILogRepository
{
    private readonly CosmosClient _client;
    private readonly Container _access;
    private readonly Container _actions;
    private readonly Container _searchResult;

    public CosmosLogRepository(string connectionString, string databaseName)
    {
        _client = new CosmosClient(connectionString);
        var database = _client.CreateDatabaseIfNotExistsAsync(databaseName).GetAwaiter().GetResult().Database;
        _access = database.CreateContainerIfNotExistsAsync("access", "/session_id").GetAwaiter().GetResult();
        _actions = database.CreateContainerIfNotExistsAsync("actions", "/session_id").GetAwaiter().GetResult();
        _searchResult = database.CreateContainerIfNotExistsAsync("search_result", "/session_id").GetAwaiter().GetResult();
    }

    public Task AddAccessLogAsync(AccessLog log)
        => _access.CreateItemAsync(log, new PartitionKey(log.SessionId.ToString()));

    public Task AddActionLogAsync(ActionLog log)
        => _actions.CreateItemAsync(log, new PartitionKey(log.SessionId.ToString()));

    public Task AddSearchResultLogAsync(SearchResultLog log)
        => _searchResult.CreateItemAsync(log, new PartitionKey(log.SessionId.ToString()));
}
