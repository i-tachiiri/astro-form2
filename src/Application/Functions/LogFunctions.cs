using System.Net;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Application.Functions;

public class LogFunctions
{
    private readonly ILogRepository _repository;
    private readonly ILogger _logger;

    public LogFunctions(ILogRepository repository, ILoggerFactory loggerFactory)
    {
        _repository = repository;
        _logger = loggerFactory.CreateLogger<LogFunctions>();
    }

    [Function("LogAccess")]
    public async Task<HttpResponseData> LogAccess(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/access")] HttpRequestData req)
    {
        var log = await req.ReadFromJsonAsync<AccessLog>();
        if (log == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        try
        {
            await _repository.AddAccessLogAsync(log);
            return req.CreateResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log access");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("LogAction")]
    public async Task<HttpResponseData> LogAction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/action")] HttpRequestData req)
    {
        var log = await req.ReadFromJsonAsync<ActionLog>();
        if (log == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        try
        {
            await _repository.AddActionLogAsync(log);
            return req.CreateResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log action");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("LogSearchResult")]
    public async Task<HttpResponseData> LogSearchResult(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/search_result")] HttpRequestData req)
    {
        var log = await req.ReadFromJsonAsync<SearchResultLog>();
        if (log == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        try
        {
            await _repository.AddSearchResultLogAsync(log);
            return req.CreateResponse(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log search result");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
