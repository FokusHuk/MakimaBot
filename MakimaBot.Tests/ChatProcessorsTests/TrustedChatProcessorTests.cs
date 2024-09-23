using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;

namespace MakimaBot.Tests;

[TestClass]
public class TrustedChatProcessorTests
{
    private CancellationToken _cancellationToken;
    private Mock<IDataContext> _dataContext;
    private long _existedChatId = 1234;

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();
        
        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.IsChatExists(_existedChatId))
            .Returns(true)
            .Verifiable();

        _dataContext
            .Setup(x => x.IsChatExists(It.IsNotIn(_existedChatId)))
            .Returns(false)
            .Verifiable();
    }

    [TestMethod]
    public async Task ReceiveMessageIn_TrustedChat_Process()
    {
        var trustedChatProcessor = new TrustedChatProcessor(_dataContext.Object);

        await trustedChatProcessor.ProcessChainAsync(message: null, _existedChatId, _cancellationToken);

        //Maybe will be some logic in ProcessAsync later
        _dataContext.Verify(x => x.IsChatExists(_existedChatId), Times.Once());
        _dataContext.Verify(x => x.IsChatExists(It.IsNotIn(_existedChatId)), Times.Never());
    } 

    [TestMethod]
    public async Task ReceiveMessageIn_UntrustedChat_DoNothing()
    {
        var trustedChatProcessor = new TrustedChatProcessor(_dataContext.Object);

        await trustedChatProcessor.ProcessChainAsync(message: null, _existedChatId + 1, _cancellationToken);

        //Maybe will be some logic in ProcessAsync later
        _dataContext.Verify(x => x.IsChatExists(_existedChatId), Times.Never());
        _dataContext.Verify(x => x.IsChatExists(It.IsNotIn(_existedChatId)), Times.Once());
    } 
}