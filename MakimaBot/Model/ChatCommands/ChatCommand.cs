using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public abstract class ChatCommand
{
    public abstract string Name { get; set;}

    public abstract Task ExecuteAsync(
        Message message,
        ChatState chatState,
        string rawParameters,
        TelegramBotClient _telegramBotClient,
        CancellationToken cancellationToken);
}
