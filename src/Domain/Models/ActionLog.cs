namespace Domain.Models;

public record ActionLog(string Id, string SessionId, string ActionName, DateTime ActionedAt);
