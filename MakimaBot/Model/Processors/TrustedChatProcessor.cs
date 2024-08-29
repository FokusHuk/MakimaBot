using MakimaBot.Model;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TrustedChatProcessor : ProcessorBase
{
    public TrustedChatProcessor(DataContext dataContext) : base(dataContext)
    {

    }

    protected override Task ExecuteBody(Message message, long chatId, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override bool ExecuteCondition(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        return chatState is not null;
    }                               
}
