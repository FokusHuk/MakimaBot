using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public class AddDailyBackupJobStateMigration : Migration
{
    public override int GetVersion() => 2;

    public override void Migrate(JObject state)
    {
        var infrastructureState = state["infrastructure"] as JObject;

        infrastructureState["dailyBackupJobState"] = new JObject()
        {
            ["lastTimeStampUtc"] = DateTime.UtcNow.AddDays(-1).ToString("o"),
        };
    }
}
