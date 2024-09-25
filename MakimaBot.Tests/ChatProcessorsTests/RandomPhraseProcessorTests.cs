using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class RandomPhraseProcessorTests
{
    private const long ExistedChatId = 1;
    private TestTelegramTextMessageSender _telegramTextMessageSender;
    private Mock<IDataContext> _dataContext;

    [TestInitialize]
    public void TestInitialize()
    {
        _telegramTextMessageSender = new TestTelegramTextMessageSender();

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
        var randomPhraseProcessor = new RandomPhraseProcessorWithoutRandom(_dataContext.Object, _telegramTextMessageSender);
     
        await randomPhraseProcessor.ProcessChainAsync(message: null, ExistedChatId, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(ExistedChatId), Times.Once());
        Assert.IsTrue(_telegramTextMessageSender.SentMessage is not null);
        Assert.IsTrue(!string.IsNullOrEmpty(_telegramTextMessageSender.SentMessage.Text));
    }
}

file class RandomPhraseProcessorWithoutRandom : RandomPhraseProcessor
{
    public RandomPhraseProcessorWithoutRandom(IDataContext dataContext, 
                                              ITelegramTextMessageSender telegramTextMessageSender) 
                                              : base(dataContext, telegramTextMessageSender)
    {

    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken) => true;
}
