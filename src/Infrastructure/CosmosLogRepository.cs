using Domain.Models;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;

namespace Infrastructure;

public class CosmosLogRepository : ILogRepository
{
    private readonly Container _accessContainer;
    private readonly Container _actionsContainer;
    private readonly Container _searchContainer;

    public CosmosLogRepository(string connectionString, string databaseName)
    {
        var client = new CosmosClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _accessContainer = database.GetContainer("access");
        _actionsContainer = database.GetContainer("actions");
        _searchContainer = database.GetContainer("search_result");
    }

    public Task AddAccessLogAsync(AccessLog log)
        => _accessContainer.CreateItemAsync(log, new PartitionKey(log.SessionId));

    public Task AddActionLogAsync(ActionLog log)
        => _actionsContainer.CreateItemAsync(log, new PartitionKey(log.SessionId));

    public Task AddSearchResultLogAsync(SearchResultLog log)
        => _searchContainer.CreateItemAsync(log, new PartitionKey(log.SessionId));
}
