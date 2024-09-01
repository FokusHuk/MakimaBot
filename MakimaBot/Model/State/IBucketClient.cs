namespace MakimaBot.Model;

public interface IBucketClient
{
    Task<string> LoadRawStateAsync();
    Task<BotState> LoadStateAsync();
    Task<bool> TryUpdateState(BotState state);
}
