using MakimaBot.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class UntrustedChatProcessor : ChatMessageProcessorBase
{
    private readonly ITelegramBotClient _telegramBotClient;

    public UntrustedChatProcessor(DataContext dataContext,
                                  ITelegramBotClient telegramBotClient) 
                                  : base(dataContext)
    {
        _telegramBotClient = telegramBotClient;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        _dataContext.AddUnknownMessage(
            sentDateTimeUtc: DateTime.UtcNow,
            chatId: chatId,
            message: message.Text,
            username: message.From?.Username ?? message.From?.FirstName ?? message.From?.LastName ?? message.From?.Id.ToString());

        await _dataContext.SaveChangesAsync();
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return !_dataContext.IsChatExists(chatId);
    }
}
