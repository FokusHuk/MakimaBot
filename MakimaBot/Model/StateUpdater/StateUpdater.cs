using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public class StateUpdater
{
    private readonly BucketClient _bucketClient;
    private readonly ITextDiffPrinter _textDiffPrinter;
    private readonly IEnumerable<Migration> _migrations;

    public StateUpdater(
        BucketClient bucketClient,
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

        var currentVersion = (int)jObjectState["stateVersion"];

        var migrationsToExecute = _migrations
            .Where(migration => migration.Version > currentVersion)
            .OrderBy(migration => migration.Version)
            .ToList();

        if (!migrationsToExecute.Any())
            return;


        migrationsToExecute.ForEach(migration => migration.Migrate(jObjectState));


        var stateAfterMigration = jObjectState.ToString();
        _textDiffPrinter.DumpDiff(stateBeforeMigration, stateAfterMigration);


        var botState = GetValidatedState(stateAfterMigration);

        botState.StateVersion = migrationsToExecute.Last().Version;

        if (!await _bucketClient.TryUpdateState(botState))
        {
            throw new InvalidOperationException("Unable to apply state changes.");
        }

        Console.WriteLine("Migrations applied successfully.");
    }

    private BotState GetValidatedState(string jsonState)
    {
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<BotState>(jsonState)
                ?? throw new InvalidOperationException("Unable to deserialize state via migrations validation.");
        }
        catch (System.Text.Json.JsonException exception)
        {
            throw new InvalidOperationException("State is invalid after applying migrations. Check for migrations.", exception);
        }
    }
}
