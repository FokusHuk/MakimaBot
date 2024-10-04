using Telegram.Bot.Types;

namespace MakimaBot.Model;

public abstract class ChatCommand
{
    public abstract string Name { get; set;}

    public abstract Task ExecuteAsync(
        Message message,
        ChatState chatState,
        string rawParameters,
        ITelegramBotClientWrapper _telegramBotClientWrapper,
        CancellationToken cancellationToken);
}
