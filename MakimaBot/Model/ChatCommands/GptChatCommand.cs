using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class GptChatCommand : ChatCommand
{
    private readonly IGptClient _gptClient;

    public override string Name { get; set; } = "gpt";

    public GptChatCommand(IGptClient gptClient)
    {
        _gptClient = gptClient;
    }

    public override async Task ExecuteAsync(
        Message message,
        ChatState chatState,
        string rawParameters,
        ITelegramTextMessageSender _telegramTextMessageSender,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rawParameters))
        {
            await _telegramTextMessageSender.SendTextMessageAsync(
                chatState.ChatId,
                "Промт не может быть пустым.",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
        }

        var response = await _gptClient.SendAsync(rawParameters);

        await _telegramTextMessageSender.SendTextMessageAsync(
                chatState.ChatId,
                response.Result.Alternatives.First().Message.Text,
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
    }
}
