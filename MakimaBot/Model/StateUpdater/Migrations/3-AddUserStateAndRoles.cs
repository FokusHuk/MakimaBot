using Newtonsoft.Json.Linq;

namespace MakimaBot.Model.StateUpdater.Migrations;

public class AddUserStateAndRolesMigration : Migration
{
    public override int GetVersion() => 3;

    public override void Migrate(JObject state)
    {
        var chats = state["chats"] as JArray;

        foreach (var chat in chats)
        {
            var usersState = new JObject
                {
                    {
                        "430654970",
                        new JObject
                        {
                            ["userName"] = "AntKhan",
                            ["userRole"] = new JObject
                            {
                                ["roleName"] = "Admin",
                                ["allowedCommands"] = JArray.FromObject(new string[] {"gpt", "list"})
                            }
                        }
                    }
                };

            chat["usersState"] = usersState;
        }
    }
}
