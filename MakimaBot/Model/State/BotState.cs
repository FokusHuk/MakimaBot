using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class BotState
{
    [JsonPropertyName("chats")]
    public required ICollection<ChatState> Chats { get; set; }
    
    [JsonPropertyName("infrastructure")]
    public required InfrastructureState Infrastructure { get; set; }
}