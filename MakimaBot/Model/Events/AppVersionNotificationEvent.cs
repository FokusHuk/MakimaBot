using System.Text.Json;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class AppVersionNotificationEvent : IChatEvent
{
    private readonly IEnumerable<Changelog> _changelogs;

    public AppVersionNotificationEvent(IEnumerable<Changelog> changelogs)
    {
        _changelogs = changelogs;
    }

     public bool ShouldLaunch(ChatState chat) => chat.EventsState.AppVersionNotification.IsEnabled;
    
    public async Task HandleEventAsync(TelegramBotClient telegramBotClient, ChatState chat)
    {   
        var changelogsToNotify = _changelogs
            .Where(c => c.Id > chat.EventsState.AppVersionNotification.LastNotifiedAppVersionId)
            .OrderByDescending(c => c.Id)
            .ToList();

        if (!changelogsToNotify.Any())
            return;

        var totalDescription = string
            .Join("\n", changelogsToNotify.Select(c => c.Description))
            .Trim();

        var lastChangelog = changelogsToNotify.First();

        var message = $"*Обновление Makima v{lastChangelog.Version}*\n{totalDescription}";

        await telegramBotClient.SendTextMessageAsync(
                chat.ChatId,
                message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

        chat.EventsState.AppVersionNotification.LastNotifiedAppVersionId = lastChangelog.Id;
    }
}
