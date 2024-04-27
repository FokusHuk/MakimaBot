using System.Text.Json.Serialization;

public class Changelog
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("version")]
    public required string Version { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }
}
