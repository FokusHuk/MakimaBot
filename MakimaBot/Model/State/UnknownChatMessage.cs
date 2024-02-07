using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class UnknownChatMessage
{
    [JsonPropertyName("sentDateTimeUtc")]
    public required DateTime SentDateTimeUtc { get; set; }
    
    [JsonPropertyName("chatId")]
    public required long ChatId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
