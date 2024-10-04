using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;

namespace MakimaBot.Tests;

[TestClass]
public class TrustedChatProcessorTests
{
    private const long ExistedChatId = 1234;
    private Mock<IDataContext> _dataContext;

    [TestInitialize]
    public void TestInitialize()
    {       
        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.IsChatExists(ExistedChatId))
            .Returns(true)
            .Verifiable();

        _dataContext
            .Setup(x => x.IsChatExists(It.IsNotIn(ExistedChatId)))
            .Returns(false)
            .Verifiable();
    }

    [TestMethod]
    public async Task ProcessChainAsync_MessageInTrustedChat_Process()
    {
        var trustedChatProcessor = new TrustedChatProcessor(_dataContext.Object);

        await trustedChatProcessor.ProcessChainAsync(message: null, ExistedChatId, CancellationToken.None);

        _dataContext.Verify(x => x.IsChatExists(ExistedChatId), Times.Once());
        _dataContext.Verify(x => x.IsChatExists(It.IsNotIn(ExistedChatId)), Times.Never());
    } 

    [TestMethod]
    public async Task ProcessChainAsync_MessageInUntrustedChat_DoNothing()
    {
        var trustedChatProcessor = new TrustedChatProcessor(_dataContext.Object);

        await trustedChatProcessor.ProcessChainAsync(message: null, ExistedChatId + 1, CancellationToken.None);

        _dataContext.Verify(x => x.IsChatExists(ExistedChatId), Times.Never());
        _dataContext.Verify(x => x.IsChatExists(It.IsNotIn(ExistedChatId)), Times.Once());
    } 
}