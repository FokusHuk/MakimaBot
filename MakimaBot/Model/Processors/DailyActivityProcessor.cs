using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class DailyActivityProcessor : ProcessorBase
{
    protected override bool _continueAnyway => true;

    public DailyActivityProcessor(DataContext dataContext) : base(dataContext)
    {

    }

    protected override async Task ExecuteBody(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);

        var chatActivityStatistics = chatState.EventsState.ActivityStatistics.Statistics;
        if (chatActivityStatistics.ContainsKey(message.From.Id))
            chatActivityStatistics[message.From.Id]++;
        else
            chatActivityStatistics[message.From.Id] = 1;

        await _dataContext.SaveChangesAsync();
    }

    protected override bool ExecuteCondition(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        return chatState.EventsState.ActivityStatistics.IsEnabled;
    }
}
