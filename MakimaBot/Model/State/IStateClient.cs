namespace MakimaBot.Model;

public interface IStateClient
{
    Task<string> LoadRawStateAsync();
    Task<BotState> LoadStateAsync();
    Task<bool> TryUpdateStateAsync(BotState state);
}
