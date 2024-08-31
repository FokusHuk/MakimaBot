using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public abstract class StateUpdaterBase<T> where T : class
{
    protected readonly IBucketClient _bucketClient;
    private readonly ITextDiffPrinter _textDiffPrinter;
    private readonly IEnumerable<Migration> _migrations;

    public StateUpdaterBase(
        IBucketClient bucketClient,
        ITextDiffPrinter textDiffPrinter,
        IEnumerable<Migration> migrations)
    {
        _bucketClient = bucketClient;
        _textDiffPrinter = textDiffPrinter;
        _migrations = migrations;
    }

    public async Task EnsureUpdateAsync(CancellationToken cancellationToken)
    {
        var jsonState = await _bucketClient.LoadRawStateAsync();
        var jObjectState = JsonConvert.DeserializeObject<JObject>(jsonState);
        var stateBeforeMigration = jObjectState.ToString();

        var currentVersion = GetCurrentVersion(jObjectState);

        var migrationsToExecute = _migrations
            .Where(migration => migration.Version > currentVersion)
            .OrderBy(migration => migration.Version)
            .ToList();

        if (!migrationsToExecute.Any())
            return;


        migrationsToExecute.ForEach(migration => migration.Migrate(jObjectState));


        var stateAfterMigration = jObjectState.ToString();
        _textDiffPrinter.DumpDiff(stateBeforeMigration, stateAfterMigration);


        var botState = GetValidatedState<T>(stateAfterMigration);

        if (!await TryUpdateStateAsync(botState, migrationsToExecute.Last().Version))
        {
            throw new InvalidOperationException("Unable to apply state changes.");
        }

        Console.WriteLine("Migrations applied successfully.");
    }

    private T GetValidatedState<T>(string jsonState)
    {
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonState)
                ?? throw new InvalidOperationException("Unable to deserialize state via migrations validation.");
        }
        catch (System.Text.Json.JsonException exception)
        {
            throw new InvalidOperationException("State is invalid after applying migrations. Check for migrations.", exception);
        }
    }

    protected abstract int GetCurrentVersion(JObject jObjectState);

    protected abstract Task<bool> TryUpdateStateAsync(T state, int newStateVersion);
}
