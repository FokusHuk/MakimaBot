using System.Text.Json.Serialization;

namespace MakimaBot.Model;

public  class InfrastructureState
{
    [JsonPropertyName("errors")]
    public required ICollection<BotError> Errors { get; set; }

    [JsonPropertyName("unknownChatMessages")]
    public required ICollection<UnknownChatMessage> UnknownChatsMessages { get; set; }

    [JsonPropertyName("dailyBackupJobState")]
    public required DailyBackupJobState? DailyBackupJobState { get; set; }
    
    [JsonPropertyName("serviceChats")]
    public required ICollection<ServiceChat> ServiceChats { get; set; }
    
    [JsonPropertyName("notificationsChatSettings")]
    public required NotificationsChatSettings? NotificationsChatSettings { get; set; }
}
