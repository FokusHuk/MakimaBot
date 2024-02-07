using MakimaBot.Model.Events;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class BotController
{
    private readonly TelegramBotClient _telegramClient;
    private readonly BucketClient _bucketClient;
    private readonly ChatEventsHandler _chatEventsHandler;
    private readonly ChatMessagesHandler _chatMessagesHandler;

    public BotController(TelegramBotClient telegramClient,
        BucketClient bucketClient,
        ChatEventsHandler chatEventsHandler,
        ChatMessagesHandler chatMessagesHandler)
    {
        _telegramClient = telegramClient;
        _bucketClient = bucketClient;
        _chatEventsHandler = chatEventsHandler;
        _chatMessagesHandler = chatMessagesHandler;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var state = await _bucketClient.LoadStateAsync();
        if (state.Errors.Count > 10)
            throw new ApplicationException("Too many exceptions occured. Check error logs in state.");
        
        var botUser = await _telegramClient.GetMeAsync(cancellationToken);
        LogOnStartMessage(botUser);
        
        await _chatMessagesHandler.TryHandleUpdatesAsync(state, cancellationToken);
        await _chatEventsHandler.TryHandleEventsAsync(state);
        
        CleanupOldErrors(state);
        CleanupOldUnknownMessages(state);

        if (state.WasUpdated || state.Chats.Any(chat => chat.WasUpdated))
        {
            await _bucketClient.TryUpdateState(state);
        }
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

    private void CleanupOldUnknownMessages(BotState state)
    {
        var currentDateTimeUtc = DateTime.UtcNow;
        var cleanupThreshold = TimeSpan.FromDays(7);

        var messagesToRemove = new List<UnknownChatMessage>();
        
        foreach (var unknownChatMessage in state.UnknownChatsMessages)
        {
            if (currentDateTimeUtc - unknownChatMessage.SentDateTimeUtc > cleanupThreshold)
                messagesToRemove.Add(unknownChatMessage);
        }

        foreach (var messageToRemove in messagesToRemove)
        {
            state.UnknownChatsMessages.Remove(messageToRemove);
        }
    }

    private void CleanupOldErrors(BotState state)
    {
        var currentDateTimeUtc = DateTime.UtcNow;
        var errorsCleanupThreshold = TimeSpan.FromHours(24);
        
        var errorsToRemove = new List<BotError>();
        
        foreach (var botError in state.Errors)
        {
            if (currentDateTimeUtc - botError.CreationDateTimeUtc > errorsCleanupThreshold)
                errorsToRemove.Add(botError);
        }
        
        foreach (var errorToRemove in errorsToRemove)
        {
            state.Errors.Remove(errorToRemove);
        }
    }
}
