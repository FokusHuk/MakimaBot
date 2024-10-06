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
        var chatCommandProcessor = new ChatCommandProcessor(
            _dataContext.Object, 
            _chatCommandHandler.Object, 
            telegramBotClientWrapper: null);

        await chatCommandProcessor.ProcessChainAsync(message, 0, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(0), Times.Once());
        _chatCommandHandler.Verify(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramBotClientWrapper>(),
                It.IsAny<CancellationToken>()
            ), Times.Once());
    }

    [TestMethod]
    [DataRow("@makiNO_daily_boD  command random   parameters  ")]
    [DataRow("Makima @makima_daily_bot command random   parameters  ")]
    [DataRow("")]
    [DataRow(null)]
    public async Task ProcessChainAsync_WrongRequest_DoNothing(string text)
    {
        var message = new Message().WithText(text);
        var chatCommandProcessor = new ChatCommandProcessor(
            _dataContext.Object, 
            _chatCommandHandler.Object, 
            telegramBotClientWrapper: null);

        await chatCommandProcessor.ProcessChainAsync(message, 0, CancellationToken.None);

        _dataContext.Verify(x => x.GetChatStateById(0), Times.Never());
        _chatCommandHandler.Verify(x => x.HandleAsync(
                It.IsAny<Message>(),
                It.IsAny<ChatState>(),
                It.IsAny<TelegramBotClientWrapper>(),
                It.IsAny<CancellationToken>()
            ), Times.Never());
    }
}
