namespace MakimaBot.Model;

public class DataContext
{
    private readonly BucketClient _bucketClient;
    private BotState? _state;

    public DataContext(BucketClient bucketClient)
    {
        _bucketClient = bucketClient;
    }

    public async Task ConfigureAsync()
    {       
        _state = await _bucketClient.LoadStateAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _bucketClient.TryUpdateState(_state);
    }

    public IEnumerable<BotError> GetAllErrors()
    {
        return _state.Infrastructure.Errors;
    }

    public void UpdateErrors(ICollection<BotError> errors)
    {
        _state.Infrastructure.Errors = errors;
    }
    
    public void AddOrUpdateError(DateTime creationDateTimeUtc, string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
            throw new ArgumentException(errorMessage);

        var error = _state.Infrastructure.Errors.SingleOrDefault(error => error.Message == errorMessage);

        if (error is not null)
        {
            error.LastSeenDateTimeUtc = creationDateTimeUtc;
            error.Count++;
            return;
        }

        if (_state.Infrastructure.Errors.Count > 50)
        {
            Console.WriteLine("Too many errors in state.");
            return;
        }
        
        _state.Infrastructure.Errors.Add(new BotError
        {
            LastSeenDateTimeUtc = creationDateTimeUtc,
            Message = errorMessage,
            Count = 1
        });
    }

    public ChatState? GetChatStateById(long chatId)
    {
        return _state.Chats.SingleOrDefault(chat => chat.ChatId == chatId);
    }

    public IEnumerable<ChatState> GetAllChatStates()
    {
        return _state.Chats;
    }

    public IEnumerable<UnknownChatMessage> GetAllUnknownChatMessages()
    {
        return _state.Infrastructure.UnknownChatsMessages;
    }

    public void UpdateUnknownChatMessages(ICollection<UnknownChatMessage> unknownChatMessages)
    {
        _state.Infrastructure.UnknownChatsMessages = unknownChatMessages;
    }

    public void AddUnknownMessage(DateTime sentDateTimeUtc, long chatId, string? message, string? username)
    {
        if (_state.Infrastructure.UnknownChatsMessages.Count > 50)
        {
            Console.WriteLine("Too many unknown messages in state.");
            return;
        }

        message ??= "Unknown message";
        username ??= "Unknown user";
        
        _state.Infrastructure.UnknownChatsMessages.Add(new UnknownChatMessage
        {
            SentDateTimeUtc = sentDateTimeUtc,
            ChatId = chatId,
            Message = message,
            Name = username
        });
    }
}
