using Newtonsoft.Json.Linq;

namespace MakimaBot.Model;

public abstract class Migration
{
    public int Version => GetVersion();

    public abstract int GetVersion();

    public abstract void Migrate(JObject state);
}
