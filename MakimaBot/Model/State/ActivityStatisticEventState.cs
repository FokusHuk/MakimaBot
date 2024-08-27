using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public abstract class ActivityStatisticsEventState
{
    [JsonPropertyName("isEnabled")]
    public required bool IsEnabled { get; set; }

    [JsonPropertyName("lastTimeStampUtc")]
    public required DateTime LastTimeStampUtc { get; set; }

    [JsonPropertyName("statistics")]
    public required Dictionary<long, int> Statistics { get; set; }
}
