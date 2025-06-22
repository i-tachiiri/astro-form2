using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure;

public class InMemoryLogRepository : ILogRepository
{
    public List<AccessLog> AccessLogs { get; } = new();
    public List<ActionLog> ActionLogs { get; } = new();
    public List<SearchResultLog> SearchResultLogs { get; } = new();

    public Task AddAccessLogAsync(AccessLog log)
    {
        AccessLogs.Add(log);
        return Task.CompletedTask;
    }

    public Task AddActionLogAsync(ActionLog log)
    {
        ActionLogs.Add(log);
        return Task.CompletedTask;
    }

    public Task AddSearchResultLogAsync(SearchResultLog log)
    {
        SearchResultLogs.Add(log);
        return Task.CompletedTask;
    }
}
