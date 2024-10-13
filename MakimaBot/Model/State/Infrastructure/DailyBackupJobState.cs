using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class DailyBackupJobState 
{
    [JsonPropertyName("targetChatId")]
    public required long TargetChatId { get; set; }

    [JsonPropertyName("lastTimeStampUtc")]
    public required DateTime LastTimeStampUtc { get; set; }
}
