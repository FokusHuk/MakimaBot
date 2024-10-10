using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class UserState
{
    [JsonPropertyName("userName")]
    public  string UserName { get; set; }

    [JsonPropertyName("userRole")]
    public  Role UserRole { get; set; }
}
