using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class ChatState
{
    [JsonPropertyName("chatId")]
    public required long ChatId { get; set; }

    /// <summary>
    /// Chat or username
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("eventsState")]
    public required EventsState EventsState { get; set; }
}
