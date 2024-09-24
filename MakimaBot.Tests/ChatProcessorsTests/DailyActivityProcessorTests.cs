using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class DailyActivityProcessorTests
{
    private static long _testChatId = 1;
    private static string _testChatName = "TestChat";
    private static long _existedUserId = 123456;
    private static int _existedUserMessagesCount = 5;

    private CancellationToken _cancellationToken;
    private ChatState _testChatState;
    private Mock<IDataContext> _dataContext;

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();

        _testChatState = new TestChatStateBuilder()
            .WithId(_testChatId)
            .WithName(_testChatName)
            .WithEventState(new TestEventsStateBuilder()
                .WithActivityStatisticsEventState(new()
                {
                    IsEnabled = true,
                    LastTimeStampUtc = DateTime.UtcNow,
                    Statistics = new()
                    {
                        { _existedUserId, _existedUserMessagesCount }
                    }
                })
                .Build())
            .Build();

        var state = new TestBotStateBuilder()
            .WithChat(_testChatState)
            .Build();

        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.GetChatStateById(_testChatId))
            .Returns(_testChatState)
            .Verifiable();

        _dataContext
            .Setup(x => x.SaveChangesAsync())
            .Verifiable();
    }

    [TestMethod]
    public async Task ProcessorReceiveMessage_ExistedUser_AddMessageToStatistics()
    {
        var dailyActivityProcessor = new DailyActivityProcessor(_dataContext.Object);
        var message = new Message()
            .AddText("test message")
            .AddSender(_existedUserId);
        var expectedTestUserMessagesCount = _existedUserMessagesCount + 1;

        await dailyActivityProcessor.ProcessChainAsync(message, _testChatId, _cancellationToken);

        var actualTestUserMessagesCount = _testChatState.EventsState.ActivityStatistics.Statistics[_existedUserId];
        _dataContext.Verify(x => x.GetChatStateById(_testChatId), Times.Exactly(2));
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Once());
        Assert.AreEqual(expectedTestUserMessagesCount, actualTestUserMessagesCount);
    }

    [TestMethod]
    public async Task ProcessorReceiveMessage_UnknownUser_CreateNewStatisticWithValueOne()
    {
        var dailyActivityProcessor = new DailyActivityProcessor(_dataContext.Object);
        var newUser = _existedUserId + 1;
        var message = new Message()
            .AddText("test message")
            .AddSender(newUser);
        var expectedNewUserMessagesCount = 1;

        await dailyActivityProcessor.ProcessChainAsync(message, _testChatId, _cancellationToken);

        var actualUserMessagesCount = _testChatState.EventsState.ActivityStatistics.Statistics[newUser];
        _dataContext.Verify(x => x.GetChatStateById(_testChatId), Times.Exactly(2));
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Once());
        Assert.AreEqual(expectedNewUserMessagesCount, actualUserMessagesCount);
    }

    [TestMethod]
    public async Task ProcessorReceiveMessage_DailyActivityDisabled_DoNothing()
    {
        _testChatState.EventsState.ActivityStatistics.IsEnabled = false;
        var dailyActivityProcessor = new DailyActivityProcessor(_dataContext.Object);
        var message = new Message()
            .AddText("test message")
            .AddSender(_existedUserId);

        await dailyActivityProcessor.ProcessChainAsync(message, _testChatId, _cancellationToken);

        _dataContext.Verify(x => x.GetChatStateById(_testChatId), Times.Once());
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Never());
    }
}
