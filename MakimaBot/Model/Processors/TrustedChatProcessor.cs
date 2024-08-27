using MakimaBot.Model;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TrustedChatProcessor : ProcessorBase
{
    protected override Task ExecuteBody(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override bool ExecuteCondition(Message message, ChatState chatState) => chatState is not null;                               
}
