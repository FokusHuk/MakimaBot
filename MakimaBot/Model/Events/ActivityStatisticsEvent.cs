using System.Globalization;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class ActivityStatisticsEvent : IChatEvent
{
    private readonly TimeSpan statisticsTimeStartUtc =
        DateTime.Parse("2023-01-01 20:00:00", CultureInfo.InvariantCulture).TimeOfDay;

    private readonly TimeSpan statisticsTimeEndUtc =
        DateTime.Parse("2023-01-01 20:30:00", CultureInfo.InvariantCulture).TimeOfDay;

    public bool ShouldLaunch(ChatState chat)
    {
        var currentDateTimeUtc = DateTime.UtcNow;

        return chat.EventsState.ActivityStatistics.IsEnabled
               && currentDateTimeUtc.Date != chat.EventsState.ActivityStatistics.LastTimeStampUtc.Date
               && currentDateTimeUtc.TimeOfDay > statisticsTimeStartUtc
               && currentDateTimeUtc.TimeOfDay < statisticsTimeEndUtc;
    }

    public async Task HandleEventAsync(TelegramBotClient telegramBotClient, ChatState chat)
    {
        var membersStatistics = "";
        foreach (var stats in chat.EventsState.ActivityStatistics.Statistics)
        {
            var member = await telegramBotClient.GetChatMemberAsync(chat.ChatId, stats.Key);
            var memberName = member.User.Username ?? member.User.FirstName;

            membersStatistics += $"{memberName}: {stats.Value}\n";
        }

        if (string.IsNullOrEmpty(membersStatistics))
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: chat.ChatId,
                text: """
                Сегодня правила не нарушали 🤤
                """);
        }
        else
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: chat.ChatId,
                text: $"""
                Подведем стастистику сами знаете чего 😌
                {membersStatistics.Trim()}
                """);
        }

        chat.EventsState.ActivityStatistics.Statistics.Clear();
        chat.EventsState.ActivityStatistics.LastTimeStampUtc = DateTime.UtcNow;
    }
}
