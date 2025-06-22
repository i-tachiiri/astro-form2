using System.Net;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Shared;

namespace Application.Functions;

public class MapFunctions
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly string _apiKey;

    public MapFunctions(HttpClient httpClient, ILoggerFactory loggerFactory)
    {
        _httpClient = httpClient;
        _logger = loggerFactory.CreateLogger<MapFunctions>();
        _apiKey = Environment.GetEnvironmentVariable("PLACES_API_KEY") ?? string.Empty;
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
            var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(query)}&key={_apiKey}";
            var httpResp = await _httpClient.GetAsync(url);
            if (!httpResp.IsSuccessStatusCode)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var json = await httpResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var predictions = doc.RootElement.GetProperty("predictions");
            var results = new List<SearchResultItem>();
            foreach (var p in predictions.EnumerateArray())
            {
                var item = new SearchResultItem(
                    p.GetProperty("place_id").GetString() ?? string.Empty,
                    p.GetProperty("structured_formatting").GetProperty("main_text").GetString() ?? string.Empty,
                    p.GetProperty("description").GetString() ?? string.Empty);
                results.Add(item);
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new SearchResults(results));
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
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={Uri.EscapeDataString(place_id)}&key={_apiKey}";
            var httpResp = await _httpClient.GetAsync(url);
            if (!httpResp.IsSuccessStatusCode)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var json = await httpResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var status = doc.RootElement.GetProperty("status").GetString();
            if (status == "NOT_FOUND")
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            if (status != "OK")
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var detail = doc.RootElement.GetProperty("result");
            var location = detail.GetProperty("geometry").GetProperty("location");
            var resultObj = new PlaceDetails(
                detail.GetProperty("place_id").GetString() ?? string.Empty,
                detail.GetProperty("name").GetString() ?? string.Empty,
                detail.GetProperty("formatted_address").GetString() ?? string.Empty,
                location.GetProperty("lat").GetDecimal(),
                location.GetProperty("lng").GetDecimal(),
                detail.GetProperty("url").GetString() ?? string.Empty);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(resultObj);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get place details");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
