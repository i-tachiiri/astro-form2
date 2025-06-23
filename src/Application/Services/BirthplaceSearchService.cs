using System.Text.Json;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.Services;

public class BirthplaceSearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogRepository _logRepository;
    private readonly ILogger<BirthplaceSearchService> _logger;
    private readonly string _apiKey;

    public BirthplaceSearchService(HttpClient httpClient, ILogRepository logRepository, ILogger<BirthplaceSearchService> logger)
    {
        _httpClient = httpClient;
        _logRepository = logRepository;
        _logger = logger;
        _apiKey = Environment.GetEnvironmentVariable("GooglePlacesApiKey") ?? string.Empty;
    }

    public async Task<SearchResults> SearchAsync(string query, string sessionId)
    {
        if (string.IsNullOrWhiteSpace(query)) throw new ArgumentException("Query is required", nameof(query));

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            var log = new ActionLog(Guid.NewGuid().ToString(), sessionId, "search", DateTime.UtcNow);
            await _logRepository.AddActionLogAsync(log);
        }

        var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(query)}&key={_apiKey}";
        var httpResp = await _httpClient.GetAsync(url);
        httpResp.EnsureSuccessStatusCode();
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
        return new SearchResults(results);
    }

    public async Task<PlaceDetails> GetPlaceDetailsAsync(string placeId, string query, string sessionId)
    {
        if (string.IsNullOrWhiteSpace(placeId)) throw new ArgumentException("placeId is required", nameof(placeId));

        var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={Uri.EscapeDataString(placeId)}&key={_apiKey}";
        var httpResp = await _httpClient.GetAsync(url);
        httpResp.EnsureSuccessStatusCode();
        var json = await httpResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var status = doc.RootElement.GetProperty("status").GetString();
        if (status == "NOT_FOUND")
        {
            throw new KeyNotFoundException("Place not found");
        }
        if (status != "OK")
        {
            throw new InvalidOperationException($"Places API returned {status}");
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

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            var log = new SearchResultLog(
                Guid.NewGuid().ToString(),
                sessionId,
                placeId,
                query,
                resultObj.Lat,
                resultObj.Lng,
                DateTime.UtcNow);
            await _logRepository.AddSearchResultLogAsync(log);
        }

        return resultObj;
    }
}
