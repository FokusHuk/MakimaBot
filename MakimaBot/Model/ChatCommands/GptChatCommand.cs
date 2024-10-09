using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class GptChatCommand : ChatCommand
{
    private readonly IGptClient _gptClient;

    public override string Name { get; protected set; } = "gpt";

    public GptChatCommand(IGptClient gptClient)
    {
        _gptClient = gptClient;
    }

    public override async Task ExecuteAsync(
        Message message,
        ChatState chatState,
        string rawParameters,
        ITelegramBotClientWrapper _telegramBotClientWrapper,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rawParameters))
        {
            await _telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                "Кажется вы забыли указать что хотели узнать🤦‍♀️ (@makima_daily_bot gpt promt)",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
        }

        var response = await _gptClient.SendAsync(rawParameters);

        await _telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                response.Result.Alternatives.First().Message.Text,
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
    }
}
