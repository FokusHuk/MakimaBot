using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class ChatCommandProcessorTests
{
    private Mock<IDataContext> _dataContext;
    private Mock<IChatCommandHandler> _chatCommandHandler;

    [TestInitialize]
    public void TestInitialize()
    {
        _dataContext = new Mock<IDataContext>();
        _dataContext
            .Setup(x => x.GetChatStateById(It.IsAny<long>()))
            .Returns(new TestChatStateBuilder().Build())
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
    public async Task ProcessChainAsync_RightRequest_ExecuteChatCommandHandler()
    {
        var message = new Message().WithText("@makima_daily_bot  command random   Parameter  ");
        var gptMessageProcessor = new ChatCommandProcessor(_dataContext.Object, _chatCommandHandler.Object, telegramBotClientWrapper: null);

        await gptMessageProcessor.ProcessChainAsync(message, 0, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(0), Times.Once());
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
        var gptMessageProcessor = new ChatCommandProcessor(_dataContext.Object, _chatCommandHandler.Object, telegramBotClientWrapper: null);

        await gptMessageProcessor.ProcessChainAsync(message, 0, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(0), Times.Never());
        _chatCommandHandler.Verify(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramBotClientWrapper>(),
                It.IsAny<CancellationToken>()
            ), Times.Never());
    }
}
