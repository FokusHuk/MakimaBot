using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class UserState
{
    [JsonPropertyName("userId")]
    public required long UserId { get; set; }

    [JsonPropertyName("userName")]
    public required string UserName { get; set; }

    [JsonPropertyName("allowedCommands")]
    public required ICollection<string> AllowedCommands { get; set; }
}
