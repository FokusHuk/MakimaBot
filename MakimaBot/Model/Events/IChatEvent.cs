using Telegram.Bot;

namespace MakimaBot.Model.Events;

public interface IChatEvent
{
    bool ShouldLaunch(ChatState chat);
    Task HandleEventAsync(ITelegramBotClient telegramBotClient, ChatState chat);
}
