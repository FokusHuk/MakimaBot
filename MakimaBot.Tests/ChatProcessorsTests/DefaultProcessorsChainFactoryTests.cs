using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class DefaultProcessorsChainFactoryTests
{
    private readonly long _existedUserId = 1234;
    private readonly long _existedChatId = 1;
    private ChatMessageProcessorBase _chain;
    private List<TestProcessorType> _executionQueue;

    [TestInitialize]
    public void TestInitialize()
    {
        var state = new TestBotStateBuilder()
            .WithChat(new TestChatStateBuilder()
                .WithId(1)
                .WithName("testName")
                .WithEventState(new TestEventsStateBuilder()
                    .WithActivityStatisticsEventState(new(){
                        IsEnabled = true,
                        LastTimeStampUtc = DateTime.UtcNow,
                        Statistics = new()
                        {
                            {_existedUserId, 5}
                        }
                    })
                    .Build())
                .Build())
            .Build();
            
        var bucketClient = new Mock<IBucketClient>();
        bucketClient
            .Setup(x => x.LoadStateAsync())
            .ReturnsAsync(state);

        var dataContext = new DataContext(bucketClient.Object);
        Task.Run(() => dataContext.ConfigureAsync()).Wait();

        _executionQueue = new List<TestProcessorType>();
        var component = new ProcessorComponent(
            dailyActivityProcessor: new TestDailyActivityProcessor(dataContext, new ProcessorsTestInfo(TestProcessorType.DailyActivity, _executionQueue)),
            gptMessageProcessor: new TestGptMessageProcessor(dataContext, null, null, new ProcessorsTestInfo(TestProcessorType.GptMessage, _executionQueue)),
            healthCheackProcessor: new TestHealthCheackProcessor(dataContext, null, new ProcessorsTestInfo(TestProcessorType.HealthCheck, _executionQueue)),
            randomPhraseProcessor: new TestRandomPhraseProcessor(dataContext, null,  new ProcessorsTestInfo(TestProcessorType.RandomPhrase, _executionQueue)),
            trustedChatProcessor: new TestTrustedChatProcessor(dataContext, new ProcessorsTestInfo(TestProcessorType.TrustedChat, _executionQueue)),
            untrustedChatProcessor: new TestUntrustedChatProcessor(dataContext, new ProcessorsTestInfo(TestProcessorType.UntrustedChat, _executionQueue))
        );
        var chainFactory = new DefaultProcessorsChainFactory(component);

        _chain = chainFactory.CreateChain();
    }

    [TestMethod]
    public async Task ReceiveMessageIn_UntrustedChat_UntrustedChatProcessorExecutedOnly()
    {
        var message = new Message()
            .AddText("Hi")
            .AddSender((int)_existedUserId + 1);

        await _chain.ProcessChainAsync(message, _existedChatId + 1, new CancellationToken());

        CollectionAssert.AreEqual(new[]{ TestProcessorType.UntrustedChat}, _executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ReceiveHealthCheckMessageIn_UntrustedChat_HealthCheckProcessorExecutedOnly()
    {
        var message = new Message()
            .AddSticker("makimapak", "😤")
            .AddSender((int)_existedUserId + 1);

        await _chain.ProcessChainAsync(message, _existedChatId + 1, new CancellationToken());

        CollectionAssert.AreEqual(new[]{ TestProcessorType.HealthCheck }, _executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ReceiveGptMessageIn_UntrustedChat_UntrustedChatProcessorExecutedOnly()
    {
        var message = new Message()
            .AddText("@makima_daily_bot gpt promt")
            .AddSender((int)_existedUserId + 1);

        await _chain.ProcessChainAsync(message, _existedChatId + 1, new CancellationToken());

        CollectionAssert.AreEqual(new[]{ TestProcessorType.UntrustedChat }, _executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ReceiveHealthCheckMessageIn_TrustedChat_HealthCheckProcessorExecutedOnly()
    {
        var message = new Message()
            .AddSticker("makimapak", "😤")
            .AddSender((int)_existedUserId);

        await _chain.ProcessChainAsync(message, _existedChatId, new CancellationToken());

        CollectionAssert.AreEqual(new[]{ TestProcessorType.HealthCheck }, _executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ReceiveGptMessageIn_TrustedChat_TrustedChatProcessorAndGptMessageProcessorExecutedOnly()
    {
        var message = new Message()
            .AddText("@makima_daily_bot gpt promt")
            .AddSender((int)_existedUserId);

        await _chain.ProcessChainAsync(message, _existedChatId, new CancellationToken());

        CollectionAssert.AreEqual(new[] { 
                TestProcessorType.TrustedChat, 
                TestProcessorType.GptMessage 
            }, 
            _executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ReceiveMessageIn_TrustedChat_TrustedChatProcessorAndDailyActivityProcessorAndRandomPhraseProcessorExecutedOnly()
    {
        var message = new Message()
            .AddText("Random message")
            .AddSender((int)_existedUserId);

        await _chain.ProcessChainAsync(message, _existedChatId, new CancellationToken());

        CollectionAssert.AreEqual(new[]{
                TestProcessorType.TrustedChat,
                TestProcessorType.DailyActivity,
                TestProcessorType.RandomPhrase
            }, 
            _executionQueue.ToArray());
    }
}
