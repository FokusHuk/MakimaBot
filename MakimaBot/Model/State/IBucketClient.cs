namespace MakimaBot.Model;

public interface IBucketClient
{
    Task<string> LoadRawStateAsync();
    Task<BotState> LoadStateAsync();
    Task<bool> TryUpdateStateAsync(BotState state);
}
