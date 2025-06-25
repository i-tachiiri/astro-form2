using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Domain.Models;

public record SearchResultLog(
    [property: JsonPropertyName("id")]
    [property: JsonProperty("id")]
    string Id,

    [property: JsonPropertyName("session_id")]
    [property: JsonProperty("session_id")]
    string SessionId,

    [property: JsonPropertyName("place_id")]
    [property: JsonProperty("place_id")]
    string PlaceId,

    [property: JsonPropertyName("query")]
    [property: JsonProperty("query")]
    string Query,

    [property: JsonPropertyName("lat")]
    [property: JsonProperty("lat")]
    decimal Lat,

    [property: JsonPropertyName("lng")]
    [property: JsonProperty("lng")]
    decimal Lng,

    [property: JsonPropertyName("searched_at")]
    [property: JsonProperty("searched_at")]
    DateTime SearchedAt);
