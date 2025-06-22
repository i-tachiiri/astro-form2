namespace Shared;

public record SearchResultItem(string PlaceId, string Name, string Description);

public record SearchResults(IReadOnlyList<SearchResultItem> Results);
