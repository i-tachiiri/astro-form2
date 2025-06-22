namespace Domain.Models;

public record SearchResultLog(
    string Id,
    string SessionId,
    string PlaceId,
    string Query,
    decimal Lat,
    decimal Lng,
    DateTime SearchedAt);
