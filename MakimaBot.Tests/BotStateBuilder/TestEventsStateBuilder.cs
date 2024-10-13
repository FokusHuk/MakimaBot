using MakimaBot.Model;

namespace MakimaBot.Tests;

public class TestEventsStateBuilder
{
    private MorningMessageEventState? _morningMessage = null;
    private ActivityStatisticsEventState? _activityStatistics = null;
    private AppVersionNotificationEventState? _appVersionNotification = null;
    private EveningMessageEventState? _eveningMessage = null;

    public TestEventsStateBuilder WithMorningMessageEventState(MorningMessageEventState morningMessage)
    {
        _morningMessage = morningMessage;
        return this;
    }

    public TestEventsStateBuilder WithActivityStatisticsEventState(ActivityStatisticsEventState activityStatistics)
    {
        _activityStatistics = activityStatistics;
        return this;
    }

    public TestEventsStateBuilder WithAppVersionNotificationEventState(AppVersionNotificationEventState appVersionNotification)
    {
        _appVersionNotification = appVersionNotification;
        return this;
    }

    public TestEventsStateBuilder WithEveningMessageEventState(EveningMessageEventState eveningMessage)
    {
        _eveningMessage = eveningMessage;
        return this;
    }

    public EventsState Build()
    {
        return new EventsState
        {
            MorningMessage = _morningMessage ?? CreateMorningMessageEventState(),
            ActivityStatistics = _activityStatistics ?? CreateActivityStatisticsEventState(),
            AppVersionNotification = _appVersionNotification ?? CreateAppVersionNotificationEventState(),
            EveningMessage = _eveningMessage ?? CreateEveningMessageEventState()
        };
    }

    private MorningMessageEventState CreateMorningMessageEventState() => new()
    {
        IsEnabled = false,
        LastTimeStampUtc = DateTime.UtcNow,
        NextStartTimeStampUtc = DateTime.UtcNow
    };

    private ActivityStatisticsEventState CreateActivityStatisticsEventState() => new()
    {
        IsEnabled = false,
        LastTimeStampUtc = DateTime.UtcNow,
        Statistics = []
    };

    private AppVersionNotificationEventState CreateAppVersionNotificationEventState() => new()
    {
        IsEnabled = false,
        LastNotifiedAppVersionId = 1
    };

    private EveningMessageEventState CreateEveningMessageEventState() => new()
    {
        IsEnabled = false,
        LastTimeStampUtc = DateTime.UtcNow,
        NextStartTimeStampUtc = DateTime.UtcNow
    };
}
