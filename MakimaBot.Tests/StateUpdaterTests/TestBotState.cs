using System.Text.Json.Serialization;

namespace MakimaBot.Tests;

public class TestBotState
{
    [JsonPropertyName("stateVersion")]
    public required int StateVersion { get; set; }

    [JsonPropertyName("data")]
    public required string Data { get; set; }

    [JsonPropertyName("entities")]
    public required IEnumerable<TestStateEntity> Entities { get; set; }
}

public class TestStateEntity
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("subEntity")]
    public required TestStateSubEntity SubEntity { get; set; }
}

public class TestStateSubEntity
{
    [JsonPropertyName("isValid")]
    public required bool IsValid { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }
}
