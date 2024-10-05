using MakimaBot.Model.Events;
using MakimaBot.Model.Infrastructure;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class BotService(
    ITelegramBotClientWrapper telegramBotClientWrapper,
    IDataContext dataContext,
    ChatEventsHandler chatEventsHandler,
    ChatMessagesHandler chatMessagesHandler,
    InfrastructureJobsHandler infrastructureJobsHandler) : IBotService
{
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        await dataContext.ConfigureAsync();

        var botUser = await telegramBotClientWrapper.GetMeAsync(cancellationToken);
        LogOnStartMessage(botUser);

        await chatMessagesHandler.TryHandleUpdatesAsync(cancellationToken);
        await chatEventsHandler.TryHandleEventsAsync();
        await infrastructureJobsHandler.TryHandleJobsAsync(cancellationToken);
        
        LogOnFinishMessage(botUser);
    }

    private void LogOnStartMessage(User user)
    {
        Console.WriteLine($"[{telegramBotClientWrapper.GetHashCode()}] Start listening for @{user.Username}.");
    }
    
    private void LogOnFinishMessage(User user)
    {
        Console.WriteLine($"[{telegramBotClientWrapper.GetHashCode()}] Stop listening for @{user.Username}");
    }
}
