using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class BotError
{   
    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("lastSeenDateTimeUtc")]
    public required DateTime LastSeenDateTimeUtc { get; set; }

    [JsonPropertyName("count")]
    public required int Count { get; set; }
}