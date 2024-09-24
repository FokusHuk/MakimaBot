using MakimaBot.Model.Processors;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class ChatMessageProcessorBaseTests
{
    [TestMethod]
    public async Task ChainedWith_FirstToSecond_OnlyFirstProcessorExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestSomeChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestSomeChatMessageProcessor(2, executionQueue, false);

        firstProcessor.ChainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[]{1}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task SubchaindeWith_FirstToSecond_BothExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestSomeChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestSomeChatMessageProcessor(2, executionQueue, false);

        firstProcessor.SubchainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[]{1, 2}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task EndChainWith_FirstToSecond_OnlyFirstProcessorExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestSomeChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestSomeChatMessageProcessor(2, executionQueue, false);

        await firstProcessor
                .EndChainWith(secondProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());
        
        CollectionAssert.AreEqual(new[]{1}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ComplexChain_FirstSubSecondToThird_FirstAndSecondProcessorsExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestSomeChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestSomeChatMessageProcessor(2, executionQueue, false);
        var thirdProcessor = new TestSomeChatMessageProcessor(3, executionQueue, false);

        await firstProcessor
                .SubchainedWith(secondProcessor)
                .EndChainWith(thirdProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());
        
        CollectionAssert.AreEqual(new[]{1, 2}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ComplexChain_FirstSubSecondAndThirdToFourth_FirstAndSecondProcessorsExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestSomeChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestSomeChatMessageProcessor(2, executionQueue, false);
        var thirdProcessor = new TestSomeChatMessageProcessor(3, executionQueue, false);
        var fourthProcessor = new TestSomeChatMessageProcessor(4, executionQueue, false);

        await firstProcessor
                .SubchainedWith(secondProcessor
                    .EndChainWith(thirdProcessor))
                .EndChainWith(fourthProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());
        
        CollectionAssert.AreEqual(new[]{1, 2}, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ContinuedAnywayChainedWith_FirstToSecond_BothExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestSomeChatMessageProcessor(1, executionQueue, true);
        var secondProcessor = new TestSomeChatMessageProcessor(2, executionQueue, false);

        firstProcessor.ChainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[] { 1, 2 }, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ComplexChainWithContinuedAnyway_FirstSubSecondToThird_FirstAndSecondProcessorsExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestSomeChatMessageProcessor(1, executionQueue, true);
        var secondProcessor = new TestSomeChatMessageProcessor(2, executionQueue, false);
        var thirdProcessor = new TestSomeChatMessageProcessor(3, executionQueue, false);

        await firstProcessor
                .SubchainedWith(secondProcessor)
                .EndChainWith(thirdProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, executionQueue.ToArray());
    }

    [TestMethod]
    public async Task ComplexChainWithChildContinuedAnyway_FirstSubSecondAndThirdToFourth_FirstToThirdProcessorsExecuted()
    {
        var executionQueue = new List<int>();
        var firstProcessor = new TestSomeChatMessageProcessor(1, executionQueue, false);
        var secondProcessor = new TestSomeChatMessageProcessor(2, executionQueue, true);
        var thirdProcessor = new TestSomeChatMessageProcessor(3, executionQueue, false);
        var fourthProcessor = new TestSomeChatMessageProcessor(4, executionQueue, false);

        await firstProcessor
                .SubchainedWith(secondProcessor
                    .EndChainWith(thirdProcessor))
                .EndChainWith(fourthProcessor)
                .ProcessChainAsync(null, 0, new CancellationToken());

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, executionQueue.ToArray());
    }
}

public class TestSomeChatMessageProcessor : ChatMessageProcessorBase
{
    private readonly bool _continueAnyway;
    public List<int> ExecutionQueue { get; set; }
    public int ProcessorId { get; set; }
    public TestSomeChatMessageProcessor(int id, List<int> executionQueue, bool continueAnyway) : base(null)
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
