using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class AppVersionNotificationEventState
{
    [JsonPropertyName("isEnabled")]
    public required bool IsEnabled { get; set; }

    [JsonPropertyName("lastNotifiedAppVersionId")]
    public required int LastNotifiedAppVersionId { get; set; }
}
