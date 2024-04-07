using System.Globalization;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class MorningMessageEvent : IChatEvent
{
    private readonly TimeSpan _morningTimeStartUtc =
        DateTime.Parse("2023-01-01 05:00:00", CultureInfo.InvariantCulture).TimeOfDay;
    
    private readonly TimeSpan _morningTimeEndUtc =
        DateTime.Parse("2023-01-01 07:25:00", CultureInfo.InvariantCulture).TimeOfDay;
    
    public bool ShouldLaunch(ChatState chat)
    {
        var currentDateTimeUtc = DateTime.UtcNow;

        return chat.EventsState.MorningMessage.IsEnabled
               && currentDateTimeUtc.Date != chat.EventsState.MorningMessage.LastTimeStampUtc.Date
               && currentDateTimeUtc.TimeOfDay > _morningTimeStartUtc
               && currentDateTimeUtc.TimeOfDay < _morningTimeEndUtc
               && currentDateTimeUtc.TimeOfDay > chat.EventsState.MorningMessage.NextStartTimeStampUtc.TimeOfDay;
    }

    public async Task HandleEventAsync(TelegramBotClient telegramBotClient, ChatState chat)
    {
        await telegramBotClient.SendTextMessageAsync(
            chatId: chat.ChatId,
            text: GetRandomMessage());

        chat.EventsState.MorningMessage.LastTimeStampUtc = DateTime.UtcNow;
        chat.EventsState.MorningMessage.NextStartTimeStampUtc = GetNextStartTimeStampUtc();
    }
    
    public DateTime GetNextStartTimeStampUtc()
    {
        var random = new Random();
        var startTicks = _morningTimeStartUtc.Ticks;
        var endTicks = _morningTimeEndUtc.Ticks - TimeSpan.FromMinutes(30).Ticks;
        var randomTicks = startTicks + (long)((endTicks - startTicks) * random.NextDouble());
        var timeOfDay = TimeSpan.FromTicks(randomTicks);

        return DateTime.UtcNow.Date.AddDays(1).Add(timeOfDay);
    }

    private string GetRandomMessage()
    {
        var random = new Random();

        return _morningMessages[random.Next(0, _morningMessages.Length)];
    }

    private readonly string[] _morningMessages = {
        "Доброе утро ❤️",
        "Доброе утро, красивые 🕊",
        "Утречко! 🐰",
        "GOOD MORNING, VIETNAAAAAAM!",
        "Good morning, sunshines :3",
        "Утро... спать хочется",
        "Хорошего дня, я дальше спать 😴",
        "Чудесного дня, друзья 🤤",
        "Доброе утро, друзья!"
    };
}
