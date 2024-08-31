using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public  class BotState
{
    [JsonPropertyName("stateVersion")]
    public required int StateVersion { get; set; }
    
    [JsonPropertyName("chats")]
    public required ICollection<ChatState> Chats { get; set; }

    [JsonPropertyName("infrastructure")]
    public required InfrastructureState Infrastructure { get; set; }
}
