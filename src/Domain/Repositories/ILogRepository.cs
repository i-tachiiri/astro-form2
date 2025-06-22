using Domain.Models;

namespace Domain.Repositories;

public interface ILogRepository
{
    Task AddAccessLogAsync(AccessLog log);
    Task AddActionLogAsync(ActionLog log);
    Task AddSearchResultLogAsync(SearchResultLog log);
}
