using System.Runtime.CompilerServices;
using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class GptMessageProcessorTests
{
    private static long _testChatId = 1;
    private static string _testChatName = "TestChat";

    private Mock<IDataContext> _dataContext;
    private Mock<IChatCommandHandler> _chatCommandHandler;
    private CancellationToken _cancellationToken;

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();

        var testChatState = new TestChatStateBuilder()
                            .WithId(_testChatId)
                            .WithName(_testChatName)
                            .Build();

        var state = new TestBotStateBuilder()
                    .WithChat(testChatState)
                    .Build();

        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.GetChatStateById(_testChatId))
            .Returns(testChatState)
            .Verifiable();

        _chatCommandHandler = new Mock<IChatCommandHandler>();
        _chatCommandHandler
            .Setup(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramTextMessageSender>(),
                It.IsAny<CancellationToken>()
            ))
            .Verifiable();
    }

    [TestMethod]
    public async Task ReceiveMessageWith_RightRequest_SendRequestToApi()
    {
        var message = new Message().AddText("@makima_daily_bot  gpt random   Promt  ");
        var gptMessageProcessor = new GptMessageProcessor(_dataContext.Object, _chatCommandHandler.Object, telegramTextMessageSender: null);

        await gptMessageProcessor.ProcessChainAsync(message, _testChatId, _cancellationToken);

        _dataContext.Verify(x => x.GetChatStateById(_testChatId), Times.Once());
        _chatCommandHandler.Verify(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramTextMessageSender>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
    }

    [TestMethod]
    public async Task ReceiveMessageWith_WrongBotName_DoNothing()
    {
        var message = new Message().AddText("@makiNO_daily_boD  gpt random   Promt  ");
        var gptMessageProcessor = new GptMessageProcessor(_dataContext.Object, _chatCommandHandler.Object, telegramTextMessageSender: null);

        await gptMessageProcessor.ProcessChainAsync(message, _testChatId, _cancellationToken);

        _dataContext.Verify(x => x.GetChatStateById(_testChatId), Times.Never());
        _chatCommandHandler.Verify(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramTextMessageSender>(),
                It.IsAny<CancellationToken>()
            ), Times.Never());
    }

    [TestMethod]
    public async Task ReceiveMessageWhich_StartsWithoutBotName_DoNothing()
    {
        var message = new Message().AddText("Makima @makima_daily_bot gpt random   Promt  ");
        var gptMessageProcessor = new GptMessageProcessor(_dataContext.Object, _chatCommandHandler.Object, telegramTextMessageSender: null);

        await gptMessageProcessor.ProcessChainAsync(message, _testChatId, _cancellationToken);

        _dataContext.Verify(x => x.GetChatStateById(_testChatId), Times.Never());
        _chatCommandHandler.Verify(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramTextMessageSender>(),
                It.IsAny<CancellationToken>()
            ), Times.Never());
    }
}
