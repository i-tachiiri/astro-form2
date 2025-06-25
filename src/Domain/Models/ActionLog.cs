using System.Text.Json.Serialization;

namespace Domain.Models;

public record ActionLog(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("session_id")] string SessionId,
    [property: JsonPropertyName("action_name")] string ActionName,
    [property: JsonPropertyName("actioned_at")] DateTime ActionedAt);
