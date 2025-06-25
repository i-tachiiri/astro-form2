using System.Text.Json.Serialization;

namespace Domain.Models;

public record SearchResultLog(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("session_id")] string SessionId,
    [property: JsonPropertyName("place_id")] string PlaceId,
    [property: JsonPropertyName("query")] string Query,
    [property: JsonPropertyName("lat")] decimal Lat,
    [property: JsonPropertyName("lng")] decimal Lng,
    [property: JsonPropertyName("searched_at")] DateTime SearchedAt);
