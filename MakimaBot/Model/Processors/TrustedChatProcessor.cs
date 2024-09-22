using MakimaBot.Model;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TrustedChatProcessor : ChatMessageProcessorBase
{
    public TrustedChatProcessor(IDataContext dataContext) : base(dataContext)
    {

    }

    protected override Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return _dataContext.IsChatExists(chatId);
    }                               
}
