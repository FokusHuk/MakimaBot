using MakimaBot.Model.Processors;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class ChatMessageProcessorBaseTests
{
    [TestMethod]
    public async Task ProcessChainAsync_FirstChainedWithSecond_OnlyFirstProcessorExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestAbstractChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestAbstractChatMessageProcessor(2, executionQueue, false);

        firstProcessor.ChainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[]{1}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_FirstSubchaindeWithSecond_BothExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestAbstractChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestAbstractChatMessageProcessor(2, executionQueue, false);

        firstProcessor.SubchainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[]{1, 2}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_FirstEndChainWithSecond_OnlyFirstProcessorExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestAbstractChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestAbstractChatMessageProcessor(2, executionQueue, false);

        await firstProcessor
                .EndChainWith(secondProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());
        
        CollectionAssert.AreEqual(new[]{1}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_ComplexChainBaseBehaviour_FirstAndSecondProcessorsExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestAbstractChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestAbstractChatMessageProcessor(2, executionQueue, false);
        var thirdProcessor = new TestAbstractChatMessageProcessor(3, executionQueue, false);

        await firstProcessor
                .SubchainedWith(secondProcessor)
                .EndChainWith(thirdProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());
        
        CollectionAssert.AreEqual(new[]{1, 2}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_ComplexChainSubProcessorBehaviour_FirstAndSecondProcessorsExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestAbstractChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestAbstractChatMessageProcessor(2, executionQueue, false);
        var thirdProcessor = new TestAbstractChatMessageProcessor(3, executionQueue, false);
        var fourthProcessor = new TestAbstractChatMessageProcessor(4, executionQueue, false);

        await firstProcessor
                .SubchainedWith(secondProcessor
                    .EndChainWith(thirdProcessor))
                .EndChainWith(fourthProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());
        
        CollectionAssert.AreEqual(new[]{1, 2}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_BaseContinuedAnywayBehaviour_BothExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestAbstractChatMessageProcessor(1, executionQueue, true);
        var secondProcessor = new TestAbstractChatMessageProcessor(2, executionQueue, false);

        firstProcessor.ChainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[] { 1, 2 }, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_ComplexChainContinuedAnywayBehaviour_FirstAndSecondProcessorsExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestAbstractChatMessageProcessor(1, executionQueue, true);
        var secondProcessor = new TestAbstractChatMessageProcessor(2, executionQueue, false);
        var thirdProcessor = new TestAbstractChatMessageProcessor(3, executionQueue, false);

        await firstProcessor
                .SubchainedWith(secondProcessor)
                .EndChainWith(thirdProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_ComplexSubChainContinuedAnywayBehaviour_FirstToThirdProcessorsExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestAbstractChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestAbstractChatMessageProcessor(2, executionQueue, true);
        var thirdProcessor = new TestAbstractChatMessageProcessor(3, executionQueue, false);
        var fourthProcessor = new TestAbstractChatMessageProcessor(4, executionQueue, false);

        await firstProcessor
                .SubchainedWith(secondProcessor
                    .EndChainWith(thirdProcessor))
                .EndChainWith(fourthProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, executionQueue.ToArray());
    }
}

public class TestAbstractChatMessageProcessor : ChatMessageProcessorBase
{
    private readonly bool _continueAnyway;
    public List<int> ExecutionQueue { get; set; }
    public int ProcessorId { get; set; }
    public TestAbstractChatMessageProcessor(int id, List<int> executionQueue, bool continueAnyway) : base(null)
    {
        ProcessorId = id;
        ExecutionQueue = executionQueue;
        _continueAnyway = continueAnyway;
    }
    public override bool СontinueAnyway => _continueAnyway;

    protected override Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            ExecutionQueue.Add(ProcessorId);
        });
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return true;
    }
}
