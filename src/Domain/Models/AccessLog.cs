using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Domain.Models;

public record AccessLog(
    [property: JsonPropertyName("id")]
    [property: JsonProperty("id")]
    string Id,

    [property: JsonPropertyName("session_id")]
    [property: JsonProperty("session_id")]
    string SessionId,

    [property: JsonPropertyName("accessed_at")]
    [property: JsonProperty("accessed_at")]
    DateTime AccessedAt);
