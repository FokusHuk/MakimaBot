namespace MakimaBot.Model;

public interface IDataContext
{
    BotState State { get; }
    Task ConfigureAsync();
    Task<bool> SaveChangesAsync();
    IEnumerable<BotError> GetAllErrors();
    void FlushErrors();
    void AddOrUpdateError(DateTime creationDateTimeUtc, string errorMessage);
    ChatState? GetChatStateById(long chatId);
    bool IsChatExists(long chatId);
    IEnumerable<ChatState> GetAllChatStates();
    IEnumerable<UnknownChatMessage> GetAllUnknownChatMessages();
    void FlushUnknownChatMessages();
    void AddUnknownMessage(DateTime sentDateTimeUtc, long chatId, string? message, string? username);
}