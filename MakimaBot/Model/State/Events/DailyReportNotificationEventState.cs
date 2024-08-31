using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class DailyReportNotificationEventState 
{
    [JsonPropertyName("isEnabled")]
    public required bool IsEnabled { get; set; }

    [JsonPropertyName("lastTimeStampUtc")]
    public required DateTime LastTimeStampUtc { get; set; }
}