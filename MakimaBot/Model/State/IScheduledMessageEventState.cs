namespace MakimaBot.Model;

public interface IScheduledMessageEventState
{
    bool IsEnabled { get; set; }

    DateTime LastTimeStampUtc { get; set; }

    DateTime NextStartTimeStampUtc { get; set; }
}
