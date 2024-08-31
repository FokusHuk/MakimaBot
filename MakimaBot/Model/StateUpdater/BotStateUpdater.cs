using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public class BotStateUpdater : StateUpdaterBase<BotState>
{
    public BotStateUpdater(
        IBucketClient bucketClient,
        ITextDiffPrinter textDiffPrinter,
        IEnumerable<Migration> migrations) : base(bucketClient, textDiffPrinter, migrations)
    {
    }

    protected override int GetCurrentVersion(JObject jObjectState) => (int)jObjectState["stateVersion"];

    protected override async Task<bool> TryUpdateStateAsync(BotState state, int newStateVersion)
    {
         state.StateVersion = newStateVersion;

        return await _bucketClient.TryUpdateState(state);
    }
}
