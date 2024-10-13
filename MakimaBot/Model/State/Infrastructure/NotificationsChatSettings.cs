using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class NotificationsChatSettings
{
    [JsonPropertyName("chatId")]
    public required long ChatId { get; set; }

    [JsonPropertyName("lastHealthCheckTimeStampUtc")]
    public required DateTime LastHealthCheckTimeStampUtc { get; set; }
}
