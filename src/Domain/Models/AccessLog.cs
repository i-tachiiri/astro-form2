namespace Domain.Models;

public record AccessLog(string Id, string SessionId, DateTime AccessedAt);
