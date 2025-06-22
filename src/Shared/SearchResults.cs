using System.Text.Json.Serialization;

namespace Shared;

public record SearchResultItem(
    [property: JsonPropertyName("place_id")] string PlaceId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description);

public record SearchResults(
    [property: JsonPropertyName("results")] IReadOnlyList<SearchResultItem> Results);
