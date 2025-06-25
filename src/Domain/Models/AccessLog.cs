using System.Text.Json.Serialization;

namespace Domain.Models;

public record AccessLog(
    [property: JsonPropertyName("id")]
    [property: JsonProperty("id")]          // ← 追加
    string Id,

    [property: JsonPropertyName("session_id")]
    [property: JsonProperty("session_id")]  // ← 追加
    string SessionId,

    [property: JsonPropertyName("accessed_at")]
    [property: JsonProperty("accessed_at")] // ← 追加
    DateTime AccessedAt);
