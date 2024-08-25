#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class EventsState : ValidatableObject
{
    [Required]
    [JsonPropertyName("morningMessageEventState")]
    public MorningMessageEventState MorningMessage { get; set; }

    [Required]
    [JsonPropertyName("activityStatisticsEventState")]
    public ActivityStatisticsEventState ActivityStatistics { get; set; }

    [Required]
    [JsonPropertyName("dailyReportNotificationEventState")]
    public DailyReportNotificationEventState DailyReportNotification { get; set; }

    [Required]
    [JsonPropertyName("appVersionNotificationEventState")]
    public AppVersionNotificationEventState AppVersionNotification { get; set; }

    [Required]
    [JsonPropertyName("eveningMessageEventState")]
    public EveningMessageEventState EveningMessage { get; set; }

    protected override IEnumerable<CompositeValidationResult> ValidateCompositeProperties() =>
        MorningMessage.Validate()
            .Concat(ActivityStatistics.Validate())
            .Concat(DailyReportNotification.Validate())
            .Concat(AppVersionNotification.Validate())
            .Concat(EveningMessage.Validate());
}
