using Telegram.Bot.Types;

namespace MakimaBot.Model.ChatCommands;

public class ListChatCommand : ChatCommand
{
    public override string Name { get; set; } = "list";

    private readonly IEnumerable<string> _commands = new string[] { "list", "gpt"};

    public override async Task ExecuteAsync(
        Message message, 
        ChatState chatState, 
        string rawParameters, 
        ITelegramBotClientWrapper telegramBotClientWrapper, 
        CancellationToken cancellationToken)
    {
        await telegramBotClientWrapper.SendTextMessageAsync(
            chatId: chatState.ChatId,
            string.Join("\n", _commands),
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken
        );
    }
}
