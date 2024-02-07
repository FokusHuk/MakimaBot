using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class BotState
{
    [JsonIgnore]
    public bool WasUpdated { get; set; } = false;
    
    [JsonPropertyName("chats")]
    public required ICollection<ChatState> Chats { get; set; }
    
    [JsonPropertyName("errors")]
    public required ICollection<BotError> Errors { get; set; }
    
    [JsonPropertyName("unknownChatMessages")]
    public required ICollection<UnknownChatMessage> UnknownChatsMessages { get; set; }
}
