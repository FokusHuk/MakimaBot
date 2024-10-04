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
        var state = new TestBotStateBuilder()
            .WithChat(new TestChatStateBuilder()
                .WithId(ExistingTestChatId)
                .WithName("TestChat")
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
    public async Task ProcessChainAsync_MessageInUntrustedChat_AddNewUnknownMesssageAndSaveChanges()
    {
        var notExistedChatId = ExistingTestChatId + 1;
        var message = new Message()
            .WithText("test")
            .WithSender(1);
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
}
