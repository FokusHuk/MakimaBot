using MakimaBot.Model;
using Newtonsoft.Json.Linq;

namespace MakimaBot.Tests;

public class TestStateUpdater : StateUpdaterBase<TestBotState>
{
    private readonly bool _updateStateResult;

    public TestBotState? State { get; private set; } = null;

    public TestStateUpdater(
        IStateClient stateClient,
        ITextDiffPrinter textDiffPrinter,
        IEnumerable<Migration> migrations,
        bool updateStateResult) : base(stateClient, textDiffPrinter, migrations)
    {
        _updateStateResult = updateStateResult;
    }

    protected override int GetCurrentVersion(JObject jObjectState) => (int)jObjectState["stateVersion"];

    protected override Task<bool> TryUpdateStateAsync(TestBotState state, int newStateVersion)
    {
        state.StateVersion = newStateVersion;
        State = state;
        return Task.FromResult(_updateStateResult);
    }
}
