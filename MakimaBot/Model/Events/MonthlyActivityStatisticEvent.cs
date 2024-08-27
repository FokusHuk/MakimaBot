using System.Globalization;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class MonthlyActivityStatisticsEvent : IChatEvent
{
    private readonly TimeSpan statisticsTimeStartUtc = new TimeSpan(hours: 20, minutes: 40, seconds: 0);

    private readonly TimeSpan statisticsTimeEndUtc = new TimeSpan(hours: 20, minutes: 59, seconds: 0);

    public bool ShouldLaunch(ChatState chat)
    {
        var currentDateTimeUtc = DateTime.UtcNow;
        int daysInMonth = DateTime.DaysInMonth(currentDateTimeUtc.Year, currentDateTimeUtc.Month);
        var lastDayOfMonth = new DateTime(currentDateTimeUtc.Year, currentDateTimeUtc.Month, daysInMonth).Date;

        return chat.EventsState.MonthlyActivityStatistics.IsEnabled
               && currentDateTimeUtc.Date == lastDayOfMonth
               && currentDateTimeUtc.TimeOfDay > statisticsTimeStartUtc
               && currentDateTimeUtc.TimeOfDay < statisticsTimeEndUtc;
    }

    public async Task HandleEventAsync(TelegramBotClient telegramBotClient, ChatState chat)
    {
        var membersStatistics = "";
        foreach (var stats in chat.EventsState.MonthlyActivityStatistics.Statistics)
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
                В этом месяце правила не нарушали 🤤 (Чат что, умер?)
                """);
        }
        else
        {
            await telegramBotClient.SendTextMessageAsync(
                chatId: chat.ChatId,
                text: $"""
                Подведем стастистику за месяц сами знаете чего 😌
                {membersStatistics.Trim()}
                """);
        }

        chat.EventsState.MonthlyActivityStatistics.Statistics.Clear();
        chat.EventsState.MonthlyActivityStatistics.LastTimeStampUtc = DateTime.UtcNow;
    }
}
