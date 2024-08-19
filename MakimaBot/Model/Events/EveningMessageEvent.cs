using System.Globalization;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class EveningMessageEvent : IChatEvent
{
    private readonly TimeSpan _eveningTimeStartUtc =
        DateTime.Parse("2023-01-01 20:30:00", CultureInfo.InvariantCulture).TimeOfDay;

    private readonly TimeSpan _eveningTimeEndUtc =
        DateTime.Parse("2023-01-01 22:30:00", CultureInfo.InvariantCulture).TimeOfDay;

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
               && currentDateTimeUtc.TimeOfDay > _eveningTimeStartUtc
               && currentDateTimeUtc.TimeOfDay < _eveningTimeEndUtc
               && currentDateTimeUtc.TimeOfDay > chat.EventsState.EveningMessage.NextStartTimeStampUtc.TimeOfDay;
    }

    public DateTime GetNextStartTimeStampUtc()
    {
        var random = new Random();
        var startTicks = _eveningTimeStartUtc.Ticks;
        var endTicks = _eveningTimeEndUtc.Ticks - TimeSpan.FromMinutes(30).Ticks;
        var randomTicks = startTicks + (long)((endTicks - startTicks) * random.NextDouble());
        var timeOfDay = TimeSpan.FromTicks(randomTicks);

        return DateTime.UtcNow.Date.AddDays(1).Add(timeOfDay);
    }

    private string GetRandomMessage()
    {
        var random = new Random();

        return _eveningMessages[random.Next(0, _eveningMessages.Length)];
    }

    private readonly string[] _eveningMessages = {
        "Спокойной ночи, пусть сны будут волшебными 🌙✨",
        "Ночь окутала мир своим спокойствием. Сладких снов! 🌌",
        "Good night, sleep tight, and dream of wonderful things 🌜💫",
        "Пусть ночь принесет умиротворение и отдых. Спокойной ночи! 🌟",
        "Сладких и спокойных снов, пусть ночь будет уютной 😴💤",
        "Закрывай глаза и погружайся в мир сновидений. Спокойной ночи! 💤🌙",
        "Ночная тишина и звезды сопровождают ваш сон. Спокойной ночи, дорогие! 🌠"
    };
}
