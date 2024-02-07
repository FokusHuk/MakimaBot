using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class BotState
{
    [JsonPropertyName("chats")]
    public required ICollection<ChatState> Chats { get; set; }
    
    [JsonPropertyName("errors")]
    public required ICollection<BotError> Errors { get; set; }
    
    [JsonPropertyName("unknownChatMessages")]
    public required ICollection<UnknownChatMessage> UnknownChatsMessages { get; set; }
}
