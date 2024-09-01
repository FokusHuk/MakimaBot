using MakimaBot.Model.Processors;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class ChatMessagesHandler
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly DataContext _dataContext;
    private readonly ProcessorsChainFactory _processorsChainFactory;

    private const int UpdateMessagesLimit = 25;

    public ChatMessagesHandler(TelegramBotClient telegramBotClient,
                               DataContext dataContext, 
                               ProcessorsChainFactory processorsChainFactory)
    {
        _telegramBotClient = telegramBotClient;
        _dataContext = dataContext;
        _processorsChainFactory = processorsChainFactory;
    }

    public async Task TryHandleUpdatesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await HandleUpdatesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling updates: {e.Message}";
            Console.WriteLine(errorMessage);
            _dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await _dataContext.SaveChangesAsync();
        }
    }

    private async Task HandleUpdatesAsync(CancellationToken cancellationToken)
    {
        var messagesOffset = 0;
        var updates = await _telegramBotClient.GetUpdatesAsync(
            offset: messagesOffset,
            limit: UpdateMessagesLimit,
            cancellationToken: cancellationToken);

        if (!updates.Any())
        {
            return;
        }

        foreach (var update in updates)
        {
            await TryHandleMessagesAsync(update, cancellationToken);
            messagesOffset = update.Id + 1;
            await _telegramBotClient.GetUpdatesAsync(offset: messagesOffset, cancellationToken: cancellationToken);
        }
    }

    private async Task TryHandleMessagesAsync(Update update, CancellationToken cancellationToken)
    {
        try
        {
            await HandleMessageAsync(update, cancellationToken);
        }
        catch (Exception e)
        {
            var errorMessage = $"An error occured while handling message: {e.Message}";
            Console.WriteLine(errorMessage);
            _dataContext.AddOrUpdateError(DateTime.UtcNow, errorMessage);
            await _dataContext.SaveChangesAsync();
        }
    }
    
    private async Task HandleMessageAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        if (message.From != null)
        {
            var processorsChain = _processorsChainFactory.CreateChain();
            
            await processorsChain.ProcessChainAsync(message, message.Chat.Id, cancellationToken);
        }
    }
}
