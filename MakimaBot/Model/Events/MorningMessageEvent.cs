using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class MorningMessageEvent : ScheduledEventBase, IChatEvent
{
    protected override TimeSpan EventTimeStartUtc { get => new TimeSpan(hours: 5, minutes: 0, seconds: 0); }
    protected override TimeSpan EventTimeEndUtc { get => new TimeSpan(hours: 7, minutes: 25, seconds: 0); }
    
    public bool ShouldLaunch(ChatState chat) => ShouldLaunch(chat.EventsState.MorningMessage);

    public async Task HandleEventAsync(ITelegramTextMessageSender telegramTextMessageSender, ChatState chat)
    {
        await telegramTextMessageSender.SendTextMessageAsync(
           chatId: chat.ChatId,
           text: GetRandomMessage());

        chat.EventsState.MorningMessage.LastTimeStampUtc = DateTime.UtcNow;
        chat.EventsState.MorningMessage.NextStartTimeStampUtc = GetNextStartTimeStampUtc();
    }

    private string GetRandomMessage()
    {
        var random = new Random();

        return _morningMessages[random.Next(0, _morningMessages.Length)];
    }

    private readonly string[] _morningMessages =
    {
        "Доброе утро ❤️",
        "Доброе утро, красивые 🕊",
        "Good morning, sunshines :3",
        "Утро... спать хочется",
        "Хорошего дня, я дальше спать 😴",
        "Чудесного дня, друзья 🤤",
        "Доброе утро, друзья!"
    };
}
