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
    private readonly string? _apiKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY");

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

        if (string.IsNullOrEmpty(_apiKey))
        {
            var res = req.CreateResponse(HttpStatusCode.InternalServerError);
            await res.WriteStringAsync("API key not configured");
            return res;
        }

        var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(query)}&key={_apiKey}";
        var apiRes = await Http.GetAsync(url);
        apiRes.EnsureSuccessStatusCode();
        var json = await apiRes.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var list = new List<SearchResultItem>();
        foreach (var p in doc.RootElement.GetProperty("predictions").EnumerateArray())
        {
            var placeId = p.GetProperty("place_id").GetString() ?? string.Empty;
            var mainText = p.GetProperty("structured_formatting").GetProperty("main_text").GetString() ?? string.Empty;
            var description = p.GetProperty("description").GetString() ?? string.Empty;
            list.Add(new SearchResultItem(placeId, mainText, description));
        }

        var results = new SearchResults(list);
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(results);
        return res;
    }

    [Function("MapDetail")]
    public async Task<HttpResponseData> DetailAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "map/{place_id}")] HttpRequestData req,
        string place_id)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            var r = req.CreateResponse(HttpStatusCode.InternalServerError);
            await r.WriteStringAsync("API key not configured");
            return r;
        }

        var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={Uri.EscapeDataString(place_id)}&fields=name,formatted_address,geometry&key={_apiKey}";
        var apiRes = await Http.GetAsync(url);
        if (!apiRes.IsSuccessStatusCode)
        {
            return req.CreateResponse(apiRes.StatusCode);
        }

        var json = await apiRes.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var result = doc.RootElement.GetProperty("result");
        var name = result.GetProperty("name").GetString() ?? string.Empty;
        var address = result.GetProperty("formatted_address").GetString() ?? string.Empty;
        var loc = result.GetProperty("geometry").GetProperty("location");
        var lat = loc.GetProperty("lat").GetDouble();
        var lng = loc.GetProperty("lng").GetDouble();
        var mapUrl = $"https://www.google.com/maps/search/?api=1&query={lat},{lng}";
        var detail = new PlaceDetails(place_id, name, address, lat, lng, mapUrl);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(detail);
        return res;
    }
}
