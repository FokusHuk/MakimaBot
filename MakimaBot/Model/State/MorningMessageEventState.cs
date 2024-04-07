using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class MorningMessageEventState
{
    [JsonPropertyName("isEnabled")]
    public required bool IsEnabled { get; set; }
    
    [JsonPropertyName("lastTimeStampUtc")]
    public required DateTime LastTimeStampUtc { get; set; }
    
    [JsonPropertyName("nextStartTimeStampUtc")]
    public required DateTime NextStartTimeStampUtc { get; set; }
}
