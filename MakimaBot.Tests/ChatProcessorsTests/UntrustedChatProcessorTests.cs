using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class UntrustedChatProcessorTests
{
    private const long ExistingTestChatId = 1;
    private Mock<IDataContext> _dataContext;

    [TestInitialize]
    public void TestInitialize()
    {
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
            .Setup(x => x.IsChatExists(ExistingTestChatId))
            .Returns(true)
            .Verifiable();
        _dataContext
            .Setup(x => x.IsChatExists(It.IsNotIn<long>(ExistingTestChatId)))
            .Returns(false);
    }

    [TestMethod]
    public async Task ProcessChainAsync_MessageInTrustedChat_DoNothing()
    {
        var message = new Message()
            .WithText("test")
            .WithSender(1);
        var untrustedChatProcessor = new UntrustedChatProcessor(_dataContext.Object);

        await untrustedChatProcessor.ProcessChainAsync(message, ExistingTestChatId, CancellationToken.None);

        _dataContext.Verify(x => x.IsChatExists(ExistingTestChatId), Times.Once());
        _dataContext.Verify(x => x.AddUnknownMessage(
                    It.IsAny<DateTime>(),
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never());
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Never());
    }

    [TestMethod]
    public async Task ProcessChainAsync_MessageInUntrustedAndNotServiceChat_AddNewUnknownMesssageAndSaveChanges()
    {
        var serviceChatId = ExistingTestChatId + 1;
        var notExistedChatId = ExistingTestChatId + 2;
        var message = new Message()
            .WithText("test")
            .WithSender(1);

        _dataContext
            .Setup(x => x.State)
            .Returns(CreateState(serviceChatId));

        var untrustedChatProcessor = new UntrustedChatProcessor(_dataContext.Object);

        await untrustedChatProcessor.ProcessChainAsync(message, notExistedChatId, CancellationToken.None);

        _dataContext.Verify(x => x.IsChatExists(notExistedChatId), Times.Once());
        _dataContext.Verify(x => x.AddUnknownMessage(
                    It.IsAny<DateTime>(),
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), Times.Once());
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Once());
    }

    [TestMethod]
    public async Task  ProcessChainAsync_MessageInUntrustedServiceChat_DoNothing()
    {
        var notExistedChatId = ExistingTestChatId + 1;
        var message = new Message()
            .WithText("test")
            .WithSender(1);

        _dataContext
            .Setup(x => x.State)
            .Returns(CreateState(notExistedChatId));

        var untrustedChatProcessor = new UntrustedChatProcessor(_dataContext.Object);

        await untrustedChatProcessor.ProcessChainAsync(message, notExistedChatId, CancellationToken.None);

        _dataContext.Verify(x => x.IsChatExists(notExistedChatId), Times.Once());
        _dataContext.Verify(x => x.AddUnknownMessage(
                    It.IsAny<DateTime>(),
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never());
        _dataContext.Verify(x => x.SaveChangesAsync(), Times.Never());
    }

    private BotState CreateState(long serviceChatId) =>
        new TestBotStateBuilder()
            .WithChat(new TestChatStateBuilder()
                .WithId(ExistingTestChatId)
                .Build())
            .WithInfrastructure(new TestInfrastructureStateBuilder()
                .WithServiceChats([new ServiceChat { Id = serviceChatId }])
                .Build())
            .Build();
}
