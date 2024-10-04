using Telegram.Bot.Types;

namespace MakimaBot.Model;

public interface IChatCommandHandler
{
    Task HandleAsync(
        Message message,
        ChatState chatState,
        ITelegramBotClientWrapper _telegramBotClientWrapper,
        CancellationToken cancellationToken);
}
