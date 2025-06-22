namespace Domain.Entities;

public record SearchResultLog
{
    public Guid Id { get; init; }
    public Guid SessionId { get; init; }
    public string PlaceId { get; init; } = string.Empty;
    public string Query { get; init; } = string.Empty;
    public decimal Lat { get; init; }
    public decimal Lng { get; init; }
    public DateTime SearchedAt { get; init; }
}
