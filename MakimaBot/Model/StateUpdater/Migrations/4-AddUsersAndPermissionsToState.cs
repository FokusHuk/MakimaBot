using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public class AddUsersAndPermissionsToState : Migration
{
    public override int GetVersion() => 4;

    public override void Migrate(JObject state)
    {
        var chats = state["chats"] as JArray;
        foreach (var chat in chats)
        {
            chat["users"] = new JArray();
        }
    }
}