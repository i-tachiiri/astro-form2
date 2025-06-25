using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Domain.Models;

public record ActionLog(
    [property: JsonPropertyName("id")]
    [property: JsonProperty("id")]
    string Id,

    [property: JsonPropertyName("session_id")]
    [property: JsonProperty("session_id")]
    string SessionId,

    [property: JsonPropertyName("action_name")]
    [property: JsonProperty("action_name")]
    string ActionName,

    [property: JsonPropertyName("actioned_at")]
    [property: JsonProperty("actioned_at")]
    DateTime ActionedAt);
