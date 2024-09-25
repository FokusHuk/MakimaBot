using MakimaBot.Model.Processors;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class ChatMessageProcessorBaseTests
{
    private List<int> _processorsExecutionOrder;
    [TestInitialize]
    public void TestInitialize()
    {
        _processorsExecutionOrder = new List<int>();
    }
    [TestMethod]
    public async Task ProcessChainAsync_FirstChainedWithSecond_OnlyFirstProcessorExecuted()
    {
        var firstProcessor = new TestChatMessageProcessor(1, _processorsExecutionOrder, false);
        var secondProcessor = new TestChatMessageProcessor(2, _processorsExecutionOrder, false);

        firstProcessor.ChainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, CancellationToken.None);

        CollectionAssert.AreEqual(new[]{1}, _processorsExecutionOrder.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_FirstSubchaindeWithSecond_BothExecuted()
    {
        var firstProcessor = new TestChatMessageProcessor(1, _processorsExecutionOrder, false);
        var secondProcessor = new TestChatMessageProcessor(2, _processorsExecutionOrder, false);

        firstProcessor.SubchainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, CancellationToken.None);

        CollectionAssert.AreEqual(new[]{1, 2}, _processorsExecutionOrder.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_FirstEndChainWithSecond_OnlyFirstProcessorExecuted()
    {
        var firstProcessor = new TestChatMessageProcessor(1, _processorsExecutionOrder, false);
        var secondProcessor = new TestChatMessageProcessor(2, _processorsExecutionOrder, false);

        await firstProcessor
                .EndChainWith(secondProcessor)
                .ProcessChainAsync(null, 0, CancellationToken.None);
        
        CollectionAssert.AreEqual(new[]{1}, _processorsExecutionOrder.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_ComplexChainBaseBehaviour_FirstAndSecondProcessorsExecuted()
    {
        var firstProcessor = new TestChatMessageProcessor(1, _processorsExecutionOrder, false);
        var secondProcessor = new TestChatMessageProcessor(2, _processorsExecutionOrder, false);
        var thirdProcessor = new TestChatMessageProcessor(3, _processorsExecutionOrder, false);

        await firstProcessor
                .SubchainedWith(secondProcessor)
                .EndChainWith(thirdProcessor)
                .ProcessChainAsync(null, 0, CancellationToken.None);
        
        CollectionAssert.AreEqual(new[]{1, 2}, _processorsExecutionOrder.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_ComplexChainSubProcessorBehaviour_FirstAndSecondProcessorsExecuted()
    {
        var firstProcessor = new TestChatMessageProcessor(1, _processorsExecutionOrder, false);
        var secondProcessor = new TestChatMessageProcessor(2, _processorsExecutionOrder, false);
        var thirdProcessor = new TestChatMessageProcessor(3, _processorsExecutionOrder, false);
        var fourthProcessor = new TestChatMessageProcessor(4, _processorsExecutionOrder, false);

        await firstProcessor
                .SubchainedWith(secondProcessor
                    .EndChainWith(thirdProcessor))
                .EndChainWith(fourthProcessor)
                .ProcessChainAsync(null, 0, CancellationToken.None);
        
        CollectionAssert.AreEqual(new[]{1, 2}, _processorsExecutionOrder.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_BaseContinuedAnywayBehaviour_BothExecuted()
    {
        var firstProcessor = new TestChatMessageProcessor(1, _processorsExecutionOrder, true);
        var secondProcessor = new TestChatMessageProcessor(2, _processorsExecutionOrder, false);

        firstProcessor.ChainedWith(secondProcessor);

        await firstProcessor.ProcessChainAsync(null, 0, CancellationToken.None);

        CollectionAssert.AreEqual(new[] { 1, 2 }, _processorsExecutionOrder.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_ComplexChainContinuedAnywayBehaviour_FirstAndSecondProcessorsExecuted()
    {
        var firstProcessor = new TestChatMessageProcessor(1, _processorsExecutionOrder, true);
        var secondProcessor = new TestChatMessageProcessor(2, _processorsExecutionOrder, false);
        var thirdProcessor = new TestChatMessageProcessor(3, _processorsExecutionOrder, false);

        await firstProcessor
                .SubchainedWith(secondProcessor)
                .EndChainWith(thirdProcessor)
                .ProcessChainAsync(null, 0, CancellationToken.None);

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, _processorsExecutionOrder.ToArray());
    }

    [TestMethod]
    public async Task ProcessChainAsync_ComplexSubChainContinuedAnywayBehaviour_FirstToThirdProcessorsExecuted()
    {
        var firstProcessor = new TestChatMessageProcessor(1, _processorsExecutionOrder, false);
        var secondProcessor = new TestChatMessageProcessor(2, _processorsExecutionOrder, true);
        var thirdProcessor = new TestChatMessageProcessor(3, _processorsExecutionOrder, false);
        var fourthProcessor = new TestChatMessageProcessor(4, _processorsExecutionOrder, false);

        await firstProcessor
                .SubchainedWith(secondProcessor
                    .EndChainWith(thirdProcessor))
                .EndChainWith(fourthProcessor)
                .ProcessChainAsync(null, 0, CancellationToken.None);

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, _processorsExecutionOrder.ToArray());
    }
}

file class TestChatMessageProcessor : ChatMessageProcessorBase
{
    private readonly bool _continueAnyway;
    public List<int> ProcessorsExecutionOrder { get; set; }
    public int ProcessorId { get; set; }
    public TestChatMessageProcessor(int id, List<int> executionQueue, bool continueAnyway) : base(null)
    {
        ProcessorId = id;
        ProcessorsExecutionOrder = executionQueue;
        _continueAnyway = continueAnyway;
    }
    protected override bool СontinueAnyway => _continueAnyway;

    protected override Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        ProcessorsExecutionOrder.Add(ProcessorId);
        return Task.CompletedTask;
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return true;
    }
}
