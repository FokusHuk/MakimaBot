namespace MakimaBot.Model;

public interface IStateProvider
{
    Task<string> LoadRawStateAsync();
    Task<BotState> LoadStateAsync();
    Task<bool> TryUpdateStateAsync(BotState state);
}
