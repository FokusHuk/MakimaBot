using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public class ConfigureNotificationsMigration : Migration
{
    public override int GetVersion() => 3;

    public override void Migrate(JObject state)
    {
        var infrastructureState = state["infrastructure"] as JObject;

        infrastructureState["serviceChats"] = new JArray();

        infrastructureState["notificationsChatSettings"] = null;
        infrastructureState["dailyBackupJobState"] = null;


        var chats = state["chats"] as JArray;
        foreach (var chat in chats)
        {
            var eventsState = chat["eventsState"] as JObject;

            eventsState.Remove("dailyReportNotificationEventState");
        }
    }
}
