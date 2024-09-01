using MakimaBot.Model;
using Newtonsoft.Json.Linq;

namespace MakimaBot.Tests;

public class TestMigration : Migration
{
    private readonly int _version;
    private readonly Action<JObject> _migrationAction;

    public bool Applied { get; private set; } = false;

    public TestMigration(int version, Action<JObject> migrationAction)
    {
        _version = version;
        _migrationAction = migrationAction;
    }

    public override int GetVersion() => _version;

    public override void Migrate(JObject state)
    {
        _migrationAction(state);
        Applied = true;
    }
}
