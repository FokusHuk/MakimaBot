using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class DailyActivityProcessor : ProcessorBase
{
    protected override bool _continueAnyway => true;

    private readonly DataContext _dataContext;
    public DailyActivityProcessor(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    protected override async Task ExecuteBody(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
        var chatActivityStatistics = chatState.EventsState.ActivityStatistics.Statistics;
        if (chatActivityStatistics.ContainsKey(message.From.Id))
            chatActivityStatistics[message.From.Id]++;
        else
            chatActivityStatistics[message.From.Id] = 1;

        await _dataContext.SaveChangesAsync();
    }

    protected override bool ExecuteCondition(Message message, ChatState chatState)
    { 
        return chatState.EventsState.ActivityStatistics.IsEnabled;
    }
}
