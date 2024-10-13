using MakimaBot.Model;
using Moq;
using Newtonsoft.Json.Linq;

namespace MakimaBot.Tests;

[TestClass]
public class StateUpdaterTests
{
    private ITextDiffPrinter _textDiffPrinter;

    [TestInitialize]
    public void TestInitialize()
    {
        _textDiffPrinter = Mock.Of<ITextDiffPrinter>();
    }

    [TestMethod]
    public async Task EnsureUpdateAsync_NoMigrations_NotApplied()
    {
        var jsonState = new TestJsonBotStateBuilder()
           .WithVersion(1)
           .WithData("data")
           .WithEntity(id: 1, name: "Entity1", subEntityIsValid: false, subEntityDescription: "Description1")
           .WithEntity(id: 2, name: "Entity2", subEntityIsValid: false, subEntityDescription: "Description2")
           .Build();

        var migrations = Enumerable.Empty<Migration>();

        var stateUpdater = CreateStateUpdater(jsonState, migrations);


        await stateUpdater.EnsureUpdateAsync(CancellationToken.None);


        Assert.IsNull(stateUpdater.State);
    }

    [TestMethod]
    public async Task EnsureUpdateAsync_NoActualMigrations_NotApplied()
    {
        var jsonState = new TestJsonBotStateBuilder()
           .WithVersion(2)
           .WithData("data")
           .WithEntity(id: 1, name: "Entity1", subEntityIsValid: false, subEntityDescription: "Description1")
           .WithEntity(id: 2, name: "Entity2", subEntityIsValid: false, subEntityDescription: "Description2")
           .Build();

        var migrations = new[] {
            new TestMigration(2, state =>
            {
                state["name"] = "TestName";
            })
        };

        var stateUpdater = CreateStateUpdater(jsonState, migrations);


        await stateUpdater.EnsureUpdateAsync(CancellationToken.None);


        Assert.IsFalse(migrations.First().Applied);
        Assert.IsNull(stateUpdater.State);
    }

    [TestMethod]
    public async Task EnsureUpdateAsync_AddSimpleField_StateUpdated()
    {
        var data = "TestData";

        var jsonState = new TestJsonBotStateBuilder()
           .WithVersion(1)
           .WithEntity(id: 1, name: "Entity1", subEntityIsValid: false, subEntityDescription: "Description1")
           .WithEntity(id: 2, name: "Entity2", subEntityIsValid: false, subEntityDescription: "Description2")
           .Build();

        var migrations = new[] {
            new TestMigration(2, state =>
            {
                state["data"] = data;
            })
        };

        var stateUpdater = CreateStateUpdater(jsonState, migrations);


        await stateUpdater.EnsureUpdateAsync(CancellationToken.None);


        Assert.IsTrue(migrations.First().Applied);
        Assert.IsNotNull(stateUpdater.State);
        Assert.AreEqual(migrations.First().Version, stateUpdater.State.StateVersion);
        Assert.AreEqual(data, stateUpdater.State.Data);
    }

    [TestMethod]
    public async Task EnsureUpdateAsync_AddComplexField_StateUpdated()
    {
        var description = "TestDescription";

        var jsonState = new TestJsonBotStateBuilder()
           .WithVersion(1)
           .WithData("data")
           .WithEntity(id: 1, name: "Entity1")
           .WithEntity(id: 2, name: "Entity2")
           .Build();

        var migrations = new[] {
            new TestMigration(2, state =>
            {
                var entities = state["entities"] as JArray;

                foreach (var entity in entities)
                {
                    entity["subEntity"] = new JObject()
                    {
                        ["isValid"] = true,
                        ["description"] = description
                    };
                }
            })
        };

        var stateUpdater = CreateStateUpdater(jsonState, migrations);


        await stateUpdater.EnsureUpdateAsync(CancellationToken.None);


        Assert.IsTrue(migrations.First().Applied);
        Assert.IsNotNull(stateUpdater.State);
        Assert.AreEqual(migrations.First().Version, stateUpdater.State.StateVersion);
        Assert.IsTrue(stateUpdater.State.Entities.All(entity =>
            entity.SubEntity.IsValid &&
            entity.SubEntity.Description == description));
    }

    [TestMethod]
    public async Task EnsureUpdateAsync_TwoActualMigrations_StateUpdated()
    {
        var data = "TestData";
        var description = "TestDescription";

        var jsonState = new TestJsonBotStateBuilder()
           .WithVersion(1)
           .WithEntity(id: 1, name: "Entity1", subEntityIsValid: false)
           .WithEntity(id: 2, name: "Entity2", subEntityIsValid: false)
           .Build();

        var migrations = new[] {
            new TestMigration(2, state =>
            {
                state["data"] = data;
            }),
            new TestMigration(3, state =>
            {
                var entities = state["entities"] as JArray;

                foreach (var entity in entities)
                {
                    var subEntity = entity["subEntity"] as JObject;
                    subEntity["description"] = description;
                }
            })
        };

        var stateUpdater = CreateStateUpdater(jsonState, migrations);


        await stateUpdater.EnsureUpdateAsync(CancellationToken.None);


        Assert.IsTrue(migrations.All(migration => migration.Applied));
        Assert.IsNotNull(stateUpdater.State);
        Assert.AreEqual(3, stateUpdater.State.StateVersion);
        Assert.AreEqual(data, stateUpdater.State.Data);
        Assert.IsTrue(stateUpdater.State.Entities.All(entity => entity.SubEntity.Description == description));
    }

    [TestMethod]
    public async Task EnsureUpdateAsync_TwoMigrations_OneActual_StateUpdated()
    {
        var data = "TestData";

        var jsonState = new TestJsonBotStateBuilder()
           .WithVersion(1)
           .WithEntity(id: 1, name: "Entity1", subEntityIsValid: false, subEntityDescription: "Description1")
           .WithEntity(id: 2, name: "Entity2", subEntityIsValid: false, subEntityDescription: "Description2")
           .Build();

        var migrations = new[] {
            new TestMigration(1, state =>
            {
                var entities = state["entities"] = null;
            }),
            new TestMigration(2, state =>
            {
                state["data"] = data;
            })
        };

        var stateUpdater = CreateStateUpdater(jsonState, migrations);


        await stateUpdater.EnsureUpdateAsync(CancellationToken.None);


        Assert.IsFalse(migrations.Single(migration => migration.Version == 1).Applied);
        Assert.IsTrue(migrations.Single(migration => migration.Version == 2).Applied);
        Assert.IsNotNull(stateUpdater.State);
        Assert.AreEqual(2, stateUpdater.State.StateVersion);
        Assert.AreEqual(data, stateUpdater.State.Data);
    }

    [TestMethod]
    public async Task EnsureUpdateAsync_InvalidMigration_ThrowsOnValidation()
    {
        var jsonState = new TestJsonBotStateBuilder()
           .WithVersion(1)
           .WithEntity(id: 1, name: "Entity1", subEntityIsValid: false, subEntityDescription: "Description1")
           .WithEntity(id: 2, name: "Entity2", subEntityIsValid: false, subEntityDescription: "Description2")
           .Build();

        var migrations = new[] {
            new TestMigration(2, state =>
            {
                state["data-1"] = "TestData";
            })
        };

        var stateUpdater = CreateStateUpdater(jsonState, migrations);

        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            async () => await stateUpdater.EnsureUpdateAsync(CancellationToken.None));

        Assert.AreEqual("State is invalid after applying migrations. Check for migrations.", exception.Message);
        Assert.IsTrue(migrations.First().Applied);
        Assert.IsNull(stateUpdater.State);
    }

    [TestMethod]
    public async Task EnsureUpdateAsync_InvalidMigration_ThrowsOnApplyingUpdate()
    {
        var jsonState = new TestJsonBotStateBuilder()
           .WithVersion(1)
           .WithEntity(id: 1, name: "Entity1", subEntityIsValid: false, subEntityDescription: "Description1")
           .WithEntity(id: 2, name: "Entity2", subEntityIsValid: false, subEntityDescription: "Description2")
           .Build();

        var migrations = new[] {
            new TestMigration(2, state =>
            {
                state["data"] = "TestData";
            })
        };

        var stateUpdater = CreateStateUpdater(jsonState, migrations, updateStateResult: false);

        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            async () => await stateUpdater.EnsureUpdateAsync(CancellationToken.None));

        Assert.AreEqual("Unable to apply state changes.", exception.Message);
        Assert.IsTrue(migrations.First().Applied);
        Assert.IsNotNull(stateUpdater.State);
    }

    private TestStateUpdater CreateStateUpdater(string jsonState, IEnumerable<Migration> migrations, bool updateStateResult = true)
    {
        var stateClientMock = new Mock<IStateClient>();
        stateClientMock
            .Setup(x => x.LoadRawStateAsync())
            .ReturnsAsync(jsonState);

        return new TestStateUpdater(
            stateClientMock.Object,
            _textDiffPrinter,
            migrations,
            updateStateResult);
    }
}
