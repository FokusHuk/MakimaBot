using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class EventsState
{
    [JsonPropertyName("morningMessageEventState")]
    public required MorningMessageEventState MorningMessage { get; set; }

    [JsonPropertyName("activityStatisticsEventState")]
    public required ActivityStatisticsEventState ActivityStatistics { get; set; }

    [JsonPropertyName("dailyReportNotificationEventState")]
    public required DailyReportNotificationEventState DailyReportNotification { get; set; }

    [JsonPropertyName("appVersionNotificationEventState")]
    public required AppVersionNotificationEventState AppVersionNotification { get; set; }

    [JsonPropertyName("eveningMessageEventState")]
    public EveningMessageEventState EveningMessage { get; set; }
}
