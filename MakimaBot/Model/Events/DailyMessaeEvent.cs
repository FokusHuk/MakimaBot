using Telegram.Bot;

namespace MakimaBot.Model.Events;

public abstract class DailyMessageEvent : IChatEvent
{
    protected abstract TimeSpan EventTimeStartUtc { get; }
    protected abstract TimeSpan EventTimeEndUtc { get; }

    protected abstract string[] EventMessages { get; }

    public async Task HandleEventAsync(TelegramBotClient telegramBotClient, ChatState chat)
    {
        await telegramBotClient.SendTextMessageAsync(
                chatId: chat.ChatId,
                text: GetRandomMessage());

        chat.EventsState.EveningMessage.LastTimeStampUtc = DateTime.UtcNow;
        chat.EventsState.EveningMessage.NextStartTimeStampUtc = GetNextStartTimeStampUtc();
    }

    public bool ShouldLaunch(ChatState chat)
    {
        var currentDateTimeUtc = DateTime.UtcNow;

        return chat.EventsState.EveningMessage.IsEnabled
               && currentDateTimeUtc.Date != chat.EventsState.EveningMessage.LastTimeStampUtc.Date
               && currentDateTimeUtc.TimeOfDay > EventTimeStartUtc
               && currentDateTimeUtc.TimeOfDay < EventTimeEndUtc
               && currentDateTimeUtc.TimeOfDay > chat.EventsState.EveningMessage.NextStartTimeStampUtc.TimeOfDay;
    }

    private DateTime GetNextStartTimeStampUtc()
    {
        var random = new Random();
        var startTicks = EventTimeStartUtc.Ticks;
        var endTicks = EventTimeEndUtc.Ticks - TimeSpan.FromMinutes(30).Ticks;
        var randomTicks = startTicks + (long)((endTicks - startTicks) * random.NextDouble());
        var timeOfDay = TimeSpan.FromTicks(randomTicks);

        return DateTime.UtcNow.Date.AddDays(1).Add(timeOfDay);
    }

    private string GetRandomMessage()
    {
        var random = new Random();
        
        return EventMessages[random.Next(0, EventMessages.Length)];
    }
}
