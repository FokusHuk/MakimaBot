using System.Text.Json;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class AppVersionNotificationEvent : IChatEvent
{
    private readonly IOptions<ChangelogOptions> _changelogoptions;

    public AppVersionNotificationEvent(IOptions<ChangelogOptions> changelogOptions)
    {
        _changelogoptions = changelogOptions;
    }

     public bool ShouldLaunch(ChatState chat) => chat.EventsState.AppVersionNotification.IsEnabled;
    
    public async Task HandleEventAsync(ITelegramBotClientWrapper telegramTextMessageSender, ChatState chat)
    {   
        var changelogsToNotify = _changelogoptions.Value.Changelogs
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

        await telegramTextMessageSender.SendTextMessageAsync(
                chat.ChatId,
                message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

        chat.EventsState.AppVersionNotification.LastNotifiedAppVersionId = lastChangelog.Id;
    }
}
