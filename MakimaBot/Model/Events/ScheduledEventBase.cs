namespace MakimaBot.Model.Events;

public abstract class ScheduledEventBase
{
    protected abstract TimeSpan EventTimeStartUtc { get; }
    protected abstract TimeSpan EventTimeEndUtc { get; }

    protected bool ShouldLaunch(ScheduledMessageEventState state)
    {
        var currentDateTimeUtc = DateTime.UtcNow;

        return state.IsEnabled
                && currentDateTimeUtc.Date != state.LastTimeStampUtc.Date
                && currentDateTimeUtc.TimeOfDay > EventTimeStartUtc
                && currentDateTimeUtc.TimeOfDay < EventTimeEndUtc
                && currentDateTimeUtc.TimeOfDay > state.NextStartTimeStampUtc.TimeOfDay;
    }

    protected DateTime GetNextStartTimeStampUtc()
    {
        var random = new Random();
        var startTicks = EventTimeStartUtc.Ticks;
        var endTicks = EventTimeEndUtc.Ticks - TimeSpan.FromMinutes(30).Ticks;
        var randomTicks = startTicks + (long)((endTicks - startTicks) * random.NextDouble());
        var timeOfDay = TimeSpan.FromTicks(randomTicks);

        return DateTime.UtcNow.Date.AddDays(1).Add(timeOfDay);
    }
}
