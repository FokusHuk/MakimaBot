using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public class TestAddMigration : Migration
{
    public override int GetVersion() => 1;

    public override void Migrate(JObject state)
    {
        var chats = state["chats"] as JArray;

        foreach (var token in chats)
        {
            var eventsState = token["eventsState"] as JObject;

            eventsState["newEventState"] = new JObject()
            {
                ["isEnabled"] = false,
                ["lastTimeStampUtc"] = DateTime.UtcNow.ToString("o"),
                ["nextStartTimeStampUtc"] = DateTime.UtcNow.AddHours(1).ToString("o")
            };
        }
    }
}
