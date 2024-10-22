using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public class BotStateUpdater : StateUpdaterBase<BotState>
{
    public BotStateUpdater(
        IStateProvider stateProvider,
        ITextDiffPrinter textDiffPrinter,
        IEnumerable<Migration> migrations) : base(stateProvider, textDiffPrinter, migrations)
    {
    }

    protected override int GetCurrentVersion(JObject jObjectState) => (int)jObjectState["stateVersion"];

    protected override async Task<bool> TryUpdateStateAsync(BotState state, int newStateVersion)
    {
         state.StateVersion = newStateVersion;

        return await _stateProvider.TryUpdateStateAsync(state);
    }
}
