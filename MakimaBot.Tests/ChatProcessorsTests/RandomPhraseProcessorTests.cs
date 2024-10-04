using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class RandomPhraseProcessorTests
{
    private const long ExistedChatId = 1;
    private TestTelegramBotClientWrapper _telegramBotClientWrapper;
    private Mock<IDataContext> _dataContext;

    [TestInitialize]
    public void TestInitialize()
    {
        _telegramBotClientWrapper = new TestTelegramBotClientWrapper();

        var testChatState = new TestChatStateBuilder()
            .WithId(ExistedChatId)
            .WithName("TestChat")
            .Build();

        var state = new TestBotStateBuilder()
            .WithChat(testChatState)
            .Build();

        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.GetChatStateById(ExistedChatId))
            .Returns(testChatState)
            .Verifiable();
    }

    [TestMethod]
    public async Task ProcessChainAsync_MessageInTrustedChat_SendRandomPhraseBack()
    {      
        var randomPhraseProcessor = new RandomPhraseProcessorWithoutRandom(_dataContext.Object, _telegramBotClientWrapper);
     
        await randomPhraseProcessor.ProcessChainAsync(message: null, ExistedChatId, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(ExistedChatId), Times.Once());
        Assert.IsTrue(_telegramBotClientWrapper.SentMessage is not null);
        Assert.IsTrue(!string.IsNullOrEmpty(_telegramBotClientWrapper.SentMessage.Text));
    }
}

file class RandomPhraseProcessorWithoutRandom : RandomPhraseProcessor
{
    public RandomPhraseProcessorWithoutRandom(
        IDataContext dataContext, 
        ITelegramBotClientWrapper telegramBotClientWrapper) 
        : base(dataContext, telegramBotClientWrapper)
    {

    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken) => true;
}
