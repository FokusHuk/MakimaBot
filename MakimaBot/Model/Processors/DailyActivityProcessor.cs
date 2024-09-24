using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class DailyActivityProcessor : ChatMessageProcessorBase
{
    public override bool СontinueAnyway => true;

    public DailyActivityProcessor(IDataContext dataContext) : base(dataContext)
    {
        
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);

        var chatActivityStatistics = chatState.EventsState.ActivityStatistics.Statistics;
        if (chatActivityStatistics.ContainsKey(message.From.Id))
            chatActivityStatistics[message.From.Id]++;
        else
            chatActivityStatistics[message.From.Id] = 1;

        await _dataContext.SaveChangesAsync();
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        return chatState.EventsState.ActivityStatistics.IsEnabled;
    }
}
