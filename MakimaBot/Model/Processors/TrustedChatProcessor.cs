using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TrustedChatProcessor : ChatMessageProcessorBase
{
    public TrustedChatProcessor(IDataContext dataContext) : base(dataContext)
    {

    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        await TryAddRoleToNewUserAsync(message, chatId, cancellationToken);
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return _dataContext.IsChatExists(chatId);
    }                               

    private async Task<bool> TryAddRoleToNewUserAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        try
        {
            await AddRoleToNewUserAsync(message, chatId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occured while adding role to new user: {ex.Message}.");
            return false;
        }
    }

    private async Task AddRoleToNewUserAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        var user = message.From;

        if(!chatState.UsersState.ContainsKey(user.Id))
        {
            chatState.UsersState[user.Id] = new UserState()
            {
                UserName = user.Username,
                UserRole = new Role()
                {
                    RoleName = "DefaultUser",
                    AllowedCommands = new string[] { "list" }   
                }
            };

            await _dataContext.SaveChangesAsync();
        }
    }
}
