using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class EventsState
{
    [JsonPropertyName("morningMessageEventState")]
    public required MorningMessageEventState MorningMessage { get; set; }

    [JsonPropertyName("eveningMessageEventState")]
    public required EveningMessageEventState EveningMessage { get; set; }

    [JsonPropertyName("dailyActivityStatisticsEventState")]
    public required DailyActivityStatisticsEventState DailyActivityStatistics { get; set; }
    
    [JsonPropertyName("monthlyActivityStatisticsEventState")]
    public required MonthlyActivityStatisticsEventState MonthlyActivityStatistics { get; set; }

    [JsonPropertyName("dailyReportNotificationEventState")]
    public required DailyReportNotificationEventState DailyReportNotification { get; set; }

    [JsonPropertyName("appVersionNotificationEventState")]
    public required AppVersionNotificationEventState AppVersionNotification { get; set; }
}
