using MakimaBot.Model.Events;
using MakimaBot.Model.Infrastructure;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class BotService : IBotService
{
    private readonly ITelegramBotClientWrapper _telegramBotClientWrapper;
    private readonly IDataContext _dataContext;
    private readonly ChatEventsHandler _chatEventsHandler;
    private readonly ChatMessagesHandler _chatMessagesHandler;
    private readonly InfrastructureJobsHandler _infrastructureJobsHandler;

    public BotService(
        ITelegramBotClientWrapper telegramClient,
        IDataContext dataContext,
        ChatEventsHandler chatEventsHandler,
        ChatMessagesHandler chatMessagesHandler,
        InfrastructureJobsHandler infrastructureJobsHandler)
    {
        _telegramBotClientWrapper = telegramClient;
        _dataContext = dataContext;
        _chatEventsHandler = chatEventsHandler;
        _chatMessagesHandler = chatMessagesHandler;
        _infrastructureJobsHandler = infrastructureJobsHandler;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        await _dataContext.ConfigureAsync();

        var botUser = await _telegramBotClientWrapper.GetMeAsync(cancellationToken);
        LogOnStartMessage(botUser);

        await _chatMessagesHandler.TryHandleUpdatesAsync(cancellationToken);
        await _chatEventsHandler.TryHandleEventsAsync();
        await _infrastructureJobsHandler.TryHandleJobsAsync(cancellationToken);
        
        LogOnFinishMessage(botUser);
    }

    private void LogOnStartMessage(User user)
    {
        Console.WriteLine($"[{_telegramBotClientWrapper.GetHashCode()}] Start listening for @{user.Username}.");
    }
    
    private void LogOnFinishMessage(User user)
    {
        Console.WriteLine($"[{_telegramBotClientWrapper.GetHashCode()}] Stop listening for @{user.Username}");
    }
}
