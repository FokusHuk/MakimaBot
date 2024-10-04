using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class HealthCheackProcessor : ChatMessageProcessorBase
{
    private ITelegramBotClientWrapper _telegramBotClientWrapper;

    public HealthCheackProcessor(
        IDataContext dataContext,
        ITelegramBotClientWrapper telegramBotClientWrapper) 
        : base(dataContext)
    {
        _telegramBotClientWrapper = telegramBotClientWrapper;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        if (message.Sticker.Emoji == "😤")
        {
            await _telegramBotClientWrapper.SendTextMessageAsync(
                chatId: chatId,
                text: "❤️",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
        }
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return message.Sticker is { SetName: { } } sticker &&
            sticker.SetName.Equals("makimapak", StringComparison.InvariantCultureIgnoreCase);
    }
}
