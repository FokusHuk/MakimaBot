using Telegram.Bot;

namespace MakimaBot.Model.Events;

public interface IChatEvent
{
    bool ShouldLaunch(ChatState chat);
    Task HandleEventAsync(ITelegramTextMessageSender telegramTextMessageSender, ChatState chat);
}
