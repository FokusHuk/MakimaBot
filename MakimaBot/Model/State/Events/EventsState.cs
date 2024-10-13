using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class EventsState
{
    [JsonPropertyName("morningMessageEventState")]
    public required MorningMessageEventState MorningMessage { get; set; }

    [JsonPropertyName("activityStatisticsEventState")]
    public required ActivityStatisticsEventState ActivityStatistics { get; set; }

    [JsonPropertyName("appVersionNotificationEventState")]
    public required AppVersionNotificationEventState AppVersionNotification { get; set; }

    [JsonPropertyName("eveningMessageEventState")]
    public required EveningMessageEventState EveningMessage { get; set; }
}
