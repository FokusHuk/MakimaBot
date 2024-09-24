using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class RandomPhraseProcessorTests
{
    private  long _chatId = 1;
    private TestTelegramTextMessageSender _telegramTextMessageSender;
    private Mock<IDataContext> _dataContext;
    private CancellationToken _cancellationToken;

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();
        _telegramTextMessageSender = new TestTelegramTextMessageSender();

        var testChatState = new TestChatStateBuilder()
            .WithId(_chatId)
            .WithName("TestChat")
            .Build();

        var state = new TestBotStateBuilder()
            .WithChat(testChatState)
            .Build();

        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.GetChatStateById(_chatId))
            .Returns(testChatState)
            .Verifiable();
    }

    [TestMethod]
    public async Task ReceiveMessageIn_TrustedChat_SendRandomPhraseBack()
    {      
        var randomPhraseProcessor = new RandomPhraseProcessorWithoutRandom(_dataContext.Object, _telegramTextMessageSender);
     
        await randomPhraseProcessor.ProcessChainAsync(message: null, _chatId, _cancellationToken);

        _dataContext.Verify(x => x.GetChatStateById(_chatId), Times.Once());
        Assert.IsTrue(_telegramTextMessageSender.MessageSent is not null);
        Assert.IsTrue(!string.IsNullOrEmpty(_telegramTextMessageSender.MessageSent.Text));
    }
}

public class RandomPhraseProcessorWithoutRandom : RandomPhraseProcessor
{
    public RandomPhraseProcessorWithoutRandom(IDataContext dataContext, 
                                              ITelegramTextMessageSender telegramTextMessageSender) 
                                              : base(dataContext, telegramTextMessageSender)
    {

    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken) => true;
}
