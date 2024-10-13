using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class UntrustedChatProcessor : ChatMessageProcessorBase
{
    public UntrustedChatProcessor(IDataContext dataContext) : base(dataContext)
    {
        
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        if (_dataContext.State.Infrastructure.ServiceChats.Any(sc => sc.Id == chatId))
            return;

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
