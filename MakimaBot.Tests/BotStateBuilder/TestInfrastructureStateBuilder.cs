using MakimaBot.Model;

namespace MakimaBot.Tests;

public class TestInfrastructureStateBuilder
{
    private ICollection<BotError> _errors = [];
    private ICollection<UnknownChatMessage> _unknownChatsMessages = [];
    private DailyBackupJobState? _dailyBackupJobState = null;
    private ICollection<ServiceChat> _serviceChats = [];
    private NotificationsChatSettings? _notificationsChatSettings = null;

    public TestInfrastructureStateBuilder WithError(BotError botError)
    {
        _errors.Add(botError);
        return this;
    }

    public TestInfrastructureStateBuilder WithUnknownChatMessage(UnknownChatMessage unknownChatMessage)
    {
        _unknownChatsMessages.Add(unknownChatMessage);
        return this;
    }

    public TestInfrastructureStateBuilder WithDailyBackupJobState(DailyBackupJobState dailyBackupJobState)
    {
        _dailyBackupJobState = dailyBackupJobState;
        return this;
    }

    public TestInfrastructureStateBuilder WithServiceChats(ICollection<ServiceChat> serviceChats)
    {
        _serviceChats = serviceChats;
        return this;
    }

    public TestInfrastructureStateBuilder WithNotificationsChatSettings(NotificationsChatSettings notificationsChatSettings)
    {
        _notificationsChatSettings = notificationsChatSettings;
        return this;
    }

    public InfrastructureState Build()
    {
        return new InfrastructureState
        {
            Errors = _errors,
            UnknownChatsMessages = _unknownChatsMessages,
            DailyBackupJobState = _dailyBackupJobState,
            ServiceChats = _serviceChats,
            NotificationsChatSettings = _notificationsChatSettings
        };
    }
}
