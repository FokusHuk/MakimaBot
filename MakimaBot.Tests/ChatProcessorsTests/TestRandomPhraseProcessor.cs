using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class TestRandomPhraseProcessor
{
    private static long _chatId = 1;
    private static Mock<IBucketClient> _bucketClientMock;
    private TestTelegramTextMessageSender _telegramTextMessageSender;
    private DataContext _dataContext;
    private CancellationToken _cancellationToken;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        var state = new TestBotStateBuilder()
            .WithChat(new TestChatStateBuilder()
                .WithId(_chatId)
                .WithName("TestChat")
                .Build())
            .Build();

        _bucketClientMock = new Mock<IBucketClient>();
        _bucketClientMock
            .Setup(x => x.LoadStateAsync())
            .ReturnsAsync(state);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();
        _telegramTextMessageSender = new TestTelegramTextMessageSender();

        _dataContext = new DataContext(_bucketClientMock.Object);
        Task.Run(async () => _dataContext.ConfigureAsync()).Wait();
    }

    [TestMethod]
    public async Task SayPhraseIn_TrustedChat_SendRandomPhraseBack()
    {      
        var randomPhraseProcessor = new RandomPhraseProcessorWithoutRandom(_dataContext, _telegramTextMessageSender);
     
        await randomPhraseProcessor.ProcessChainAsync(message: null, _chatId, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is not null);
        Assert.IsTrue(!string.IsNullOrEmpty(_telegramTextMessageSender.MessageSent.Text));
    }
}

public class RandomPhraseProcessorWithoutRandom : RandomPhraseProcessor
{
    public RandomPhraseProcessorWithoutRandom(DataContext dataContext, 
                                              ITelegramTextMessageSender telegramTextMessageSender) 
                                              : base(dataContext, telegramTextMessageSender)
    {

    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken) => true;
}