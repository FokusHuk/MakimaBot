using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public class DailyBackupJobState 
{
    [JsonPropertyName("lastTimeStampUtc")]
    public required DateTime LastTimeStampUtc { get; set; }
}
