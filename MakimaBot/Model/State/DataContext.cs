namespace MakimaBot.Model;

public class DataContext
{
    private readonly IBucketClient _bucketClient;
    public BotState State { get; private set; }

    public DataContext(IBucketClient bucketClient)
    {
        _bucketClient = bucketClient;
    }

    public async Task ConfigureAsync()
    {       
        State = await _bucketClient.LoadStateAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _bucketClient.TryUpdateState(State);
    }

    public IEnumerable<BotError> GetAllErrors()
    {
        return State.Infrastructure.Errors;
    }

    public void UpdateErrors(ICollection<BotError> errors)
    {
        State.Infrastructure.Errors = errors;
    }
    
    public void AddOrUpdateError(DateTime creationDateTimeUtc, string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
            throw new ArgumentException(errorMessage);

        var error = State.Infrastructure.Errors.SingleOrDefault(error => error.Message == errorMessage);

        if (error is not null)
        {
            error.LastSeenDateTimeUtc = creationDateTimeUtc;
            error.Count++;
            return;
        }

        if (State.Infrastructure.Errors.Count > 50)
        {
            Console.WriteLine("Too many errors in state.");
            return;
        }
        
        State.Infrastructure.Errors.Add(new BotError
        {
            LastSeenDateTimeUtc = creationDateTimeUtc,
            Message = errorMessage,
            Count = 1
        });
    }

    public ChatState? GetChatStateById(long chatId)
    {
        return State.Chats.SingleOrDefault(chat => chat.ChatId == chatId);
    }

    public IEnumerable<ChatState> GetAllChatStates()
    {
        return State.Chats;
    }

    public IEnumerable<UnknownChatMessage> GetAllUnknownChatMessages()
    {
        return State.Infrastructure.UnknownChatsMessages;
    }

    public void UpdateUnknownChatMessages(ICollection<UnknownChatMessage> unknownChatMessages)
    {
        State.Infrastructure.UnknownChatsMessages = unknownChatMessages;
    }

    public void AddUnknownMessage(DateTime sentDateTimeUtc, long chatId, string? message, string? username)
    {
        if (State.Infrastructure.UnknownChatsMessages.Count > 50)
        {
            Console.WriteLine("Too many unknown messages in state.");
            return;
        }

        message ??= "Unknown message";
        username ??= "Unknown user";
        
        State.Infrastructure.UnknownChatsMessages.Add(new UnknownChatMessage
        {
            SentDateTimeUtc = sentDateTimeUtc,
            ChatId = chatId,
            Message = message,
            Name = username
        });
    }
}
