using System.Text.Json.Serialization;

namespace Shared;

public record PlaceDetails(
    [property: JsonPropertyName("place_id")] string PlaceId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lng")] double Lng,
    [property: JsonPropertyName("map_url")] string MapUrl);
