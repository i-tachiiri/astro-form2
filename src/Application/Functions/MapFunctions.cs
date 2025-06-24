using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Shared;
using Application.Services;
using System.Diagnostics.CodeAnalysis;

namespace Application.Functions;

[ExcludeFromCodeCoverage]
public class MapFunctions
{
    private readonly BirthplaceSearchService _service;
    private readonly ILogger _logger;

    public MapFunctions(BirthplaceSearchService service, ILoggerFactory loggerFactory)
    {
        _service = service;
        _logger = loggerFactory.CreateLogger<MapFunctions>();
    }

    [Function("SearchMap")]
    public async Task<HttpResponseData> SearchMap(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "map")] HttpRequestData req)
    {
        var queryParams = QueryHelpers.ParseQuery(req.Url.Query);
        if (!queryParams.TryGetValue("query", out var query) || string.IsNullOrWhiteSpace(query))
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        try
        {
            var results = await _service.SearchAsync(query!, string.Empty);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(results);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search places");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    [Function("GetPlaceDetails")]
    public async Task<HttpResponseData> GetPlaceDetails(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "map/{place_id}")] HttpRequestData req,
        string place_id)
    {
        if (string.IsNullOrWhiteSpace(place_id))
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        try
        {
            var resultObj = await _service.GetPlaceDetailsAsync(place_id, string.Empty, string.Empty);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(resultObj);
            return response;
        }
        catch (KeyNotFoundException)
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get place details");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
