using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class ChatCommandProcessorTests
{
    private const long ExistedChatId = 1;
    private Mock<IDataContext> _dataContext;
    private Mock<IChatCommandHandler> _chatCommandHandler;

    [TestInitialize]
    public void TestInitialize()
    {
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

        _chatCommandHandler = new Mock<IChatCommandHandler>();
        _chatCommandHandler
            .Setup(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramBotClientWrapper>(),
                It.IsAny<CancellationToken>()
            ))
            .Verifiable();
    }

    [TestMethod]
    public async Task ProcessChainAsync_RightRequest_SendRequestToApi()
    {
        var message = new Message().WithText("@makima_daily_bot  gpt random   Promt  ");
        var gptMessageProcessor = new ChatCommandProcessor(_dataContext.Object, _chatCommandHandler.Object, telegramTextMessageSender: null);

        await gptMessageProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(ExistedChatId), Times.Once());
        _chatCommandHandler.Verify(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramBotClientWrapper>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
    }

    [TestMethod]
    [DataRow("@makiNO_daily_boD  gpt random   Promt  ")]
    [DataRow("Makima @makima_daily_bot gpt random   Promt  ")]
    [DataRow("")]
    [DataRow(null)]
    public async Task ProcessChainAsync_WrongRequest_DoNothing(string text)
    {
        var message = new Message().WithText(text);
        var gptMessageProcessor = new ChatCommandProcessor(_dataContext.Object, _chatCommandHandler.Object, telegramTextMessageSender: null);

        await gptMessageProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(ExistedChatId), Times.Never());
        _chatCommandHandler.Verify(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramBotClientWrapper>(),
                It.IsAny<CancellationToken>()
            ), Times.Never());
    }
}
