using Newtonsoft.Json.Linq;

namespace MakimaBot.Tests;

public class TestJsonBotStateBuilder
{
    private readonly JObject _stateJObject;
    private readonly JArray _entitiesArray;

    public TestJsonBotStateBuilder()
    {
        _stateJObject = [];
        _entitiesArray = [];
    }

    public TestJsonBotStateBuilder WithVersion(int version)
    {
        _stateJObject["stateVersion"] = version;
        return this;
    }

    public TestJsonBotStateBuilder WithData(string data)
    {
        _stateJObject["data"] = data;
        return this;
    }

    public TestJsonBotStateBuilder WithEntity(
        int id,
        string name = null,
        bool? subEntityIsValid = null,
        string subEntityDescription = null)
    {
        var entityObject = new JObject
        {
            ["id"] = id
        };

        if (name != null)
        {
            entityObject["name"] = name;
        }

        if (subEntityIsValid.HasValue || subEntityDescription != null)
        {
            var subEntityObject = new JObject();

            if (subEntityIsValid.HasValue)
            {
                subEntityObject["isValid"] = subEntityIsValid.Value;
            }

            if (subEntityDescription != null)
            {
                subEntityObject["description"] = subEntityDescription;
            }

            entityObject["subEntity"] = subEntityObject;
        }

        _entitiesArray.Add(entityObject);
        return this;
    }

    public string Build()
    {
        if (_entitiesArray.Any())
        {
            _stateJObject["entities"] = _entitiesArray;
        }

        return _stateJObject.ToString();
    }
}
