using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class DailyActivityProcessorTests
{
    private const long ExistedChatId = 1;
    private const long ExistedUserId = 123456;
    private const int ExistedUserMessagesCount = 5;

    private ChatState _testChatState;
    private Mock<IDataContext> _dataContext;

    [TestInitialize]
    public void TestInitialize()
    {
        _testChatState = new TestChatStateBuilder()
            .WithId(ExistedChatId)
            .WithName("TestChat")
            .WithEventState(new TestEventsStateBuilder()
                .WithActivityStatisticsEventState(new()
                {
                    IsEnabled = true,
                    LastTimeStampUtc = DateTime.UtcNow,
                    Statistics = new()
                    {
                        { ExistedUserId, ExistedUserMessagesCount }
                    }
                })
                .Build())
            .Build();

        var state = new TestBotStateBuilder()
            .WithChat(_testChatState)
            .Build();

        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.GetChatStateById(ExistedChatId))
            .Returns(_testChatState)
            .Verifiable();

        _dataContext
            .Setup(x => x.SaveChangesAsync())
            .Verifiable();
    }

    [TestMethod]
    public async Task ProcessChainAsync_ExistedUser_AddMessageToStatistics()
    {
        var dailyActivityProcessor = new DailyActivityProcessor(_dataContext.Object);
        var message = new Message()
            .WithText("test message")
            .WithSender(ExistedUserId);
        var expectedTestUserMessagesCount = ExistedUserMessagesCount + 1;

        await dailyActivityProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        var actualTestUserMessagesCount = _testChatState.EventsState.ActivityStatistics.Statistics[ExistedUserId];
        _dataContext.Verify(x => x.GetChatStateById(ExistedChatId), Times.Exactly(2));
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Once());
        Assert.AreEqual(expectedTestUserMessagesCount, actualTestUserMessagesCount);
    }

    [TestMethod]
    public async Task ProcessChainAsync_UnknownUser_CreateNewStatisticWithValueOne()
    {
        var dailyActivityProcessor = new DailyActivityProcessor(_dataContext.Object);
        var newUserId = ExistedUserId + 1;
        var message = new Message()
            .WithText("test message")
            .WithSender(newUserId);
        var expectedNewUserMessagesCount = 1;

        await dailyActivityProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        var actualUserMessagesCount = _testChatState.EventsState.ActivityStatistics.Statistics[newUserId];
        _dataContext.Verify(x => x.GetChatStateById(ExistedChatId), Times.Exactly(2));
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Once());
        Assert.AreEqual(expectedNewUserMessagesCount, actualUserMessagesCount);
    }

    [TestMethod]
    public async Task ProcessChainAsync_DailyActivityDisabled_DoNothing()
    {
        _testChatState.EventsState.ActivityStatistics.IsEnabled = false;
        var dailyActivityProcessor = new DailyActivityProcessor(_dataContext.Object);
        var message = new Message()
            .WithText("test message")
            .WithSender(ExistedUserId);

        await dailyActivityProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(ExistedChatId), Times.Once());
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Never());
    }
}
