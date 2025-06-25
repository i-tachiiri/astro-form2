using System.Text.Json.Serialization;

namespace Domain.Models;

public record AccessLog(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("session_id")] string SessionId,
    [property: JsonPropertyName("accessed_at")] DateTime AccessedAt);
