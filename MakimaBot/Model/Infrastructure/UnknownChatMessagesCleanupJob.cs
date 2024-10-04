namespace MakimaBot.Model.Infrastructure;

public class UnknownChatMessagesCleanupJob : InfrastructureJob
{
    public override string Name => $"{nameof(UnknownChatMessagesCleanupJob)}";
    
    public override async Task ExecuteAsync(IDataContext dataContext)
    {
        var currentDateTimeUtc = DateTime.UtcNow;
        var cleanupThreshold = TimeSpan.FromDays(7);

        var unknownChatMessages = dataContext.GetAllUnknownChatMessages().ToList();
        var messagesToRemove = new List<UnknownChatMessage>();

        foreach (var unknownChatMessage in unknownChatMessages)
        {
            if (currentDateTimeUtc - unknownChatMessage.SentDateTimeUtc > cleanupThreshold)
                messagesToRemove.Add(unknownChatMessage);
        }

        foreach (var messageToRemove in messagesToRemove)
        {
            unknownChatMessages.Remove(messageToRemove);
        }

        if (messagesToRemove.Any())
        {
            dataContext.UpdateUnknownChatMessages(unknownChatMessages);
            await dataContext.SaveChangesAsync();
        }
    }
}
