using MakimaBot.Model.Events;
using MakimaBot.Model.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class BotService : IBotService
{
    private readonly ITelegramBotClient _telegramClient;
    private readonly DataContext _dataContext;
    private readonly ChatEventsHandler _chatEventsHandler;
    private readonly ChatMessagesHandler _chatMessagesHandler;
    private readonly InfrastructureJobsHandler _infrastructureJobsHandler;

    public BotService(
        ITelegramBotClient telegramClient,
        DataContext dataContext,
        ChatEventsHandler chatEventsHandler,
        ChatMessagesHandler chatMessagesHandler,
        InfrastructureJobsHandler infrastructureJobsHandler)
    {
        _telegramClient = telegramClient;
        _dataContext = dataContext;
        _chatEventsHandler = chatEventsHandler;
        _chatMessagesHandler = chatMessagesHandler;
        _infrastructureJobsHandler = infrastructureJobsHandler;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        await _dataContext.ConfigureAsync();

        var botUser = await _telegramClient.GetMeAsync(cancellationToken);
        LogOnStartMessage(botUser);

        await _chatMessagesHandler.TryHandleUpdatesAsync(cancellationToken);
        await _chatEventsHandler.TryHandleEventsAsync();
        await _infrastructureJobsHandler.TryHandleJobsAsync(cancellationToken);
        
        LogOnFinishMessage(botUser);
    }

    private void LogOnStartMessage(User user)
    {
        Console.WriteLine($"[{_telegramClient.GetHashCode()}] Start listening for @{user.Username}.");
    }
    
    private void LogOnFinishMessage(User user)
    {
        Console.WriteLine($"[{_telegramClient.GetHashCode()}] Stop listening for @{user.Username}");
    }
}
