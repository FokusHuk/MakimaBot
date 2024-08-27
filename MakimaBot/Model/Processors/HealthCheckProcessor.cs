using MakimaBot.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class HealthCheackProcessor : ProcessorBase
{
    private TelegramBotClient _telegramBotClient;

    private long _chatId;

    public HealthCheackProcessor(TelegramBotClient telegramBotClient, long chatId)
    {
        _telegramBotClient = telegramBotClient;
        _chatId = chatId;
    }

    protected override async Task ExecuteBody(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
        if (message.Sticker.Emoji == "😤")
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatId: _chatId,
                text: "❤️",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
        }
    }

    protected override bool ExecuteCondition(Message message, ChatState chatState)
    {
        return message.Sticker is { SetName: { } } sticker &&
            sticker.SetName.Equals("makimapak", StringComparison.InvariantCultureIgnoreCase);
    }
}
