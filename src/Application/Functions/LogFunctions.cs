using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Functions;

public class LogFunctions
{
    private readonly ILogger _logger;
    private readonly ILogRepository _repository;

    public LogFunctions(ILoggerFactory loggerFactory, ILogRepository repository)
    {
        _logger = loggerFactory.CreateLogger<LogFunctions>();
        _repository = repository;
    }

    [Function("AccessLog")]
    public async Task<HttpResponseData> AddAccessAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/access")] HttpRequestData req)
    {
        var log = await req.ReadFromJsonAsync<AccessLog>();
        if (log is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        await _repository.AddAccessLogAsync(log);
        return req.CreateResponse(HttpStatusCode.OK);
    }

    [Function("ActionLog")]
    public async Task<HttpResponseData> AddActionAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/action")] HttpRequestData req)
    {
        var log = await req.ReadFromJsonAsync<ActionLog>();
        if (log is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        await _repository.AddActionLogAsync(log);
        return req.CreateResponse(HttpStatusCode.OK);
    }

    [Function("SearchResultLog")]
    public async Task<HttpResponseData> AddSearchResultAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/search_result")] HttpRequestData req)
    {
        var log = await req.ReadFromJsonAsync<SearchResultLog>();
        if (log is null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        await _repository.AddSearchResultLogAsync(log);
        return req.CreateResponse(HttpStatusCode.OK);
    }
}
