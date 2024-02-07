using System.Globalization;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class MorningMessageEvent : IChatEvent
{
    private readonly TimeSpan _morningTimeStartUtc =
        DateTime.Parse("2023-01-01 05:00:00", CultureInfo.InvariantCulture).TimeOfDay;
    
    private readonly TimeSpan _morningTimeEndUtc =
        DateTime.Parse("2023-01-01 05:25:00", CultureInfo.InvariantCulture).TimeOfDay;
    
    public bool ShouldLaunch(ChatState chat)
    {
        var currentDateTimeUtc = DateTime.UtcNow;

        return chat.EventsState.MorningMessage.IsEnabled
               && currentDateTimeUtc.Date != chat.EventsState.MorningMessage.LastTimeStampUtc.Date
               && currentDateTimeUtc.TimeOfDay > _morningTimeStartUtc
               && currentDateTimeUtc.TimeOfDay < _morningTimeEndUtc;
    }

    public async Task HandleEventAsync(TelegramBotClient telegramBotClient, ChatState chat)
    {
        await telegramBotClient.SendTextMessageAsync(
            chatId: chat.ChatId,
            text: "Доброе утро ❤️");

        chat.EventsState.MorningMessage.LastTimeStampUtc = DateTime.UtcNow;
        chat.WasUpdated = true;
    }
}
