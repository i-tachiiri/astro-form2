namespace Domain.Entities;

public record AccessLog
{
    public Guid Id { get; init; }
    public Guid SessionId { get; init; }
    public DateTime AccessedAt { get; init; }
}
