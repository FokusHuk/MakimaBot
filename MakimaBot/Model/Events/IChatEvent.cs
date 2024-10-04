using Telegram.Bot;

namespace MakimaBot.Model.Events;

public interface IChatEvent
{
    bool ShouldLaunch(ChatState chat);
    Task HandleEventAsync(ITelegramBotClientWrapper telegramTextMessageSender, ChatState chat);
}
