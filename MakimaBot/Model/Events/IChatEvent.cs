using Telegram.Bot;

namespace MakimaBot.Model.Events;

public interface IChatEvent
{
    bool ShouldLaunch(ChatState chat);
    Task HandleEventAsync(ITelegramBotClientWrapper telegramBotClientWrapper, ChatState chat);
}
