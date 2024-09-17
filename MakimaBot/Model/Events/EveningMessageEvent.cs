using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class EveningMessageEvent : ScheduledEventBase, IChatEvent
{
    protected override TimeSpan EventTimeStartUtc { get => new TimeSpan(hours: 20, minutes: 30, seconds: 0); }
    protected override TimeSpan EventTimeEndUtc { get => new TimeSpan(hours: 22, minutes: 0, seconds: 0); }
    
    public bool ShouldLaunch(ChatState chat) => ShouldLaunch(chat.EventsState.EveningMessage);

    public async Task HandleEventAsync(ITelegramTextMessageSender telegramTextMessageSender, ChatState chat)
    {
        await telegramTextMessageSender.SendTextMessageAsync(
           chatId: chat.ChatId,
           text: GetRandomMessage());

        chat.EventsState.EveningMessage.LastTimeStampUtc = DateTime.UtcNow;
        chat.EventsState.EveningMessage.NextStartTimeStampUtc = GetNextStartTimeStampUtc();
    }

    private string GetRandomMessage()
    {
        var random = new Random();

        return _eveningMessages[random.Next(0, _eveningMessages.Length)];
    }

    private readonly string[] _eveningMessages =
    {
        "Спокойной ночи, пусть сны будут волшебными 🌙✨",
        "Ночь окутала мир своим спокойствием. Сладких снов! 🌌",
        "Good night, sleep tight, and dream of wonderful things 🌜💫",
        "Пусть ночь принесет умиротворение и отдых. Спокойной ночи! 🌟",
        "Сладких и спокойных снов, пусть ночь будет уютной 😴💤",
        "Закрывай глаза и погружайся в мир сновидений. Спокойной ночи! 💤🌙",
        "Ночная тишина и звезды сопровождают ваш сон. Спокойной ночи, дорогие! 🌠"
    };
}
