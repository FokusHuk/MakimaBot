using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class UntrustedChatProcessorTests
{
    private static long _existingTestChatId = 1;
    private static string _testChatName = "TestChat";

    private CancellationToken _cancellationToken;
    private Mock<IDataContext> _dataContext;

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();

        var state = new TestBotStateBuilder()
            .WithChat(new TestChatStateBuilder()
                .WithId(_existingTestChatId)
                .WithName(_testChatName)
                .Build())
            .WithInfrastructure(new TestInfrastructureStateBuilder()
                .Build())
            .Build();

        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(true)
            .Verifiable();
        _dataContext
            .Setup(x => x.AddUnknownMessage(
                It.IsAny<DateTime>(),
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Verifiable();
        _dataContext
            .Setup(x => x.IsChatExists(_existingTestChatId))
            .Returns(true)
            .Verifiable();
        _dataContext
            .Setup(x => x.IsChatExists(It.IsNotIn<long>(_existingTestChatId)))
            .Returns(false);
    }

    [TestMethod]
    public async Task UserSendMessageIn_TrustedChat_DoNothing()
    {
        var message = new Message()
            .AddText("test")
            .AddSender(1);
        var untrustedChatProcessor = new UntrustedChatProcessor(_dataContext.Object);

        await untrustedChatProcessor.ProcessChainAsync(message, _existingTestChatId, _cancellationToken);

        _dataContext.Verify(x => x.IsChatExists(_existingTestChatId), Times.Once());
        _dataContext.Verify(x => x.AddUnknownMessage(
                    It.IsAny<DateTime>(),
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never());
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Never());
    }

    [TestMethod]
    public async Task UserSendMessageIn_UntrustedChat_AddNewUnknownMesssageAndSaveChanges()
    {
        var notExistedChatId = _existingTestChatId + 1;
        var message = new Message()
            .AddText("test")
            .AddSender(1);
        var untrustedChatProcessor = new UntrustedChatProcessor(_dataContext.Object);

        await untrustedChatProcessor.ProcessChainAsync(message, notExistedChatId, _cancellationToken);

        _dataContext.Verify(x => x.IsChatExists(notExistedChatId), Times.Once());
        _dataContext.Verify(x => x.AddUnknownMessage(
                    It.IsAny<DateTime>(),
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), Times.Once());
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Once());
    }
}
