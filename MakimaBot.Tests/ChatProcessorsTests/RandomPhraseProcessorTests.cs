using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class RandomPhraseProcessorTests
{
    private static long _chatId = 1;
    
    private TestTelegramTextMessageSender _telegramTextMessageSender;
    private Mock<IDataContext> _dataContext;
    private CancellationToken _cancellationToken;

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();
        _telegramTextMessageSender = new TestTelegramTextMessageSender();

        var state = new TestBotStateBuilder()
            .WithChat(new TestChatStateBuilder()
                .WithId(_chatId)
                .WithName("TestChat")
                .Build())
            .Build();

        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.GetChatStateById(_chatId))
            .Returns(state.Chats.SingleOrDefault(x => x.ChatId == _chatId))
            .Verifiable();
    }

    [TestMethod]
    public async Task UserSendMessageIn_TrustedChat_SendRandomPhraseBack()
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
