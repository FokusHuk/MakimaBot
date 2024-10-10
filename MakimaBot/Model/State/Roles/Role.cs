using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class Role
{
    [JsonPropertyName("roleName")]
    public  string RoleName { get; set; }

    [JsonPropertyName("allowedCommands")]
    public  ICollection<string> AllowedCommands { get; set; } 
}
