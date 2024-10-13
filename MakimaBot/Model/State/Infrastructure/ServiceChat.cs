using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class ServiceChat
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }
}
