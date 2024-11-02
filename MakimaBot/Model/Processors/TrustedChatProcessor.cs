using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TrustedChatProcessor : ChatMessageProcessorBase
{
    public TrustedChatProcessor(IDataContext dataContext) : base(dataContext)
    {

    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        await TryAddNewUserStateAsync(message, chatId, cancellationToken);
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return _dataContext.IsChatExists(chatId);
    }

    private async Task<bool> TryAddNewUserStateAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        try
        {
            await AddNewUserStateAsync(message, chatId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured while adding new user to state: {ex.Message}.");
            return false;
        }
    }

    private async Task AddNewUserStateAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        var existingUserIds = chatState.Users.Select(user => user.UserId);
        var currentUser = message.From;

        if (!existingUserIds.Contains(currentUser.Id))
        {
            chatState.Users.Add(
                new UserState
                {
                    UserId = currentUser.Id,
                    UserName = currentUser.Username,
                    AllowedCommands = new string[] { "list", "help", "me"}
                }
            );

            await _dataContext.SaveChangesAsync();
        }
    }
}
