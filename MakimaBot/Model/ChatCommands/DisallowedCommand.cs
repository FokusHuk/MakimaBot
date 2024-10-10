using Telegram.Bot.Types;

namespace MakimaBot.Model.ChatCommands;

public class DisallowedCommand : ChatCommand
{
    public override string Name { get; protected set; } = "disallowedCommand";

    public override async Task ExecuteAsync(Message message, ChatState chatState, string rawParameters, ITelegramBotClientWrapper telegramBotClientWrapper, CancellationToken cancellationToken)
    {
        await telegramBotClientWrapper.SendTextMessageAsync(
            chatId: chatState.ChatId,
            "TEST - This is disallowed command! Congrats! If you see this -> you did something wrong",
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken
        );
    }
}
