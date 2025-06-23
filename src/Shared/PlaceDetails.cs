namespace Shared;

public record PlaceDetails(
    string PlaceId,
    string Name,
    string Address,
    decimal Lat,
    decimal Lng,
    string MapUrl);
