namespace Shared;

public record AccessLogDto(Guid Id, Guid SessionId, DateTime AccessedAt);

public record ActionLogDto(Guid Id, Guid SessionId, string ActionName, DateTime ActionedAt);

public record SearchResultLogDto(
    Guid Id,
    Guid SessionId,
    string PlaceId,
    string Query,
    decimal Lat,
    decimal Lng,
    DateTime SearchedAt);

