using System.Globalization;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class DailyActivityStatisticsEvent : IChatEvent
{
    private readonly TimeSpan statisticsTimeStartUtc = new TimeSpan(hours: 20, minutes: 0, seconds: 0);

    private readonly TimeSpan statisticsTimeEndUtc = new TimeSpan(hours: 20, minutes: 30, seconds: 0);

    public bool ShouldLaunch(ChatState chat)
    {
        var currentDateTimeUtc = DateTime.UtcNow;

        return chat.EventsState.DailyActivityStatistics.IsEnabled
               && currentDateTimeUtc.Date != chat.EventsState.DailyActivityStatistics.LastTimeStampUtc.Date
               && currentDateTimeUtc.TimeOfDay > statisticsTimeStartUtc
               && currentDateTimeUtc.TimeOfDay < statisticsTimeEndUtc;
    }

    public async Task HandleEventAsync(TelegramBotClient telegramBotClient, ChatState chat)
    {
        var membersStatistics = "";
        foreach (var stats in chat.EventsState.DailyActivityStatistics.Statistics)
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

        chat.EventsState.DailyActivityStatistics.Statistics.Clear();
        chat.EventsState.DailyActivityStatistics.LastTimeStampUtc = DateTime.UtcNow;
    }
}
