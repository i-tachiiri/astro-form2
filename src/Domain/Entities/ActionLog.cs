namespace Domain.Entities;

public record ActionLog
{
    public Guid Id { get; init; }
    public Guid SessionId { get; init; }
    public string ActionName { get; init; } = string.Empty;
    public DateTime ActionedAt { get; init; }
}
