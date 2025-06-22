using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Functions;

public class MapFunctions
{
    private readonly ILogger _logger;
    private static readonly HttpClient Http = new();

    public MapFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MapFunctions>();
    }

    [Function("MapSearch")]
    public async Task<HttpResponseData> SearchAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "map")] HttpRequestData req)
    {
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query).Get("query");
        if (string.IsNullOrWhiteSpace(query))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Missing query parameter");
            return bad;
        }

        // Placeholder implementation
        var results = new SearchResults(new List<SearchResultItem>
        {
            new("demo", $"{query} name", $"Description for {query}")
        });

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(results);
        return res;
    }

    [Function("MapDetail")]
    public async Task<HttpResponseData> DetailAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "map/{place_id}")] HttpRequestData req,
        string place_id)
    {
        // Placeholder detail data
        var detail = new PlaceDetails(place_id, "Demo Place", "Tokyo", 35.0, 139.0, "https://maps.example.com");
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(detail);
        return res;
    }
}
