using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class BotError
{
    [JsonPropertyName("creationDateTimeUtc")]
    public required DateTime CreationDateTimeUtc { get; set; }
    
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}