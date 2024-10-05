using MakimaBot.Model;
using Telegram.Bot.Types;

namespace MakimaBot.Tests.ChatCommandHandlerTests;

[TestClass]
public class ChatCommandHandlerTests
{
    private readonly List<ChatCommand> _chatCommands =
        new List<ChatCommand> { new TestCommand(), new TestGptCommand() };

    private TestTelegramBotClientWrapper _telegramBotClientWrapper;
    private ChatCommandHandler _chatCommandHandler;

    [TestInitialize]
    public void TestInitialize()
    {
        _telegramBotClientWrapper = new TestTelegramBotClientWrapper();
        _chatCommandHandler = new ChatCommandHandler(_chatCommands);

    }

    [TestMethod]
    [DataRow("@makima_daily_bot ")]
    [DataRow("@makima_daily_bot привет")]
    [DataRow("@makima_daily_bot гпт")]
    [DataRow("@makima_daily_bot")]
    [DataRow("@makima_daily_botgpt")]
    [DataRow("Makima @makima_daily_bot gpt promt")]
    public async Task HandleAsync_MentionOnly_SendUserError(string text)
    {
        var message = new Message().WithText(text);

        await _chatCommandHandler.HandleAsync(message, new TestChatStateBuilder().Build(), _telegramBotClientWrapper, CancellationToken.None);

        CollectionAssert.AreEqual(
            Enumerable.Repeat(0, _chatCommands.Count).ToArray(), 
            _chatCommands.Select(x => (x as TestChatCommand).ExecutionCount).ToArray());
        
        Assert.AreEqual(
            """
            @makima\_daily\_bot это команда! 
            Запросите список доступных команд ( `@makima_daily_bot list` )
            """,
            _telegramBotClientWrapper.SentMessage.Text);
    }

    [TestMethod]
    public async Task HandleAsync_UnknownCommand_SendUserError()
    {
        var message = new Message().WithText("@makima_daily_bot unknownCommand");

        await _chatCommandHandler.HandleAsync(message, new TestChatStateBuilder().Build(), _telegramBotClientWrapper, CancellationToken.None);

        CollectionAssert.AreEqual(
            Enumerable.Repeat(0, _chatCommands.Count).ToArray(), 
            _chatCommands.Select(x => (x as TestChatCommand).ExecutionCount).ToArray());
        
        Assert.AreEqual(
            """
            @makima\_daily\_bot это команда! Команда <**unknownCommand**> не распознана!
            Запросите список доступных команд ( `@makima_daily_bot list` )
            """,
            _telegramBotClientWrapper.SentMessage.Text);
    }

    [TestMethod]
    [DataRow("@makima_daily_bot test")]
    [DataRow("@makima_daily_bot test  ")]
    [DataRow("@makima_daily_bot    test")]
    [DataRow("@makima_daily_bot    test    ")]
    [DataRow("@makima_daily_bot test привет мир")]
    [DataRow("@makima_daily_bot test    привет мир")]
    public async Task HandleAsync_TestChatCommand_ExecuteTestCommand(string text)
    {
        var message = new Message().WithText(text);
    
        await _chatCommandHandler.HandleAsync(message, new TestChatStateBuilder().Build(), _telegramBotClientWrapper, CancellationToken.None);

        var testCommand = _chatCommands.Single(x => x is TestCommand) as TestChatCommand;
        Assert.AreEqual(1, testCommand.ExecutionCount);
        Assert.AreEqual(null, _telegramBotClientWrapper.SentMessage);
    }

    [TestMethod]
    public async Task HandleAsync_GptChatCommand_ExecuteCommand()
    {
        var message = new Message().WithText("@makima_daily_bot gpt promt промт");

        await _chatCommandHandler.HandleAsync(message, new TestChatStateBuilder().Build(), _telegramBotClientWrapper, CancellationToken.None);

        var testCommand = _chatCommands.Single(x => x is TestGptCommand) as TestChatCommand;
        Assert.AreEqual(1, testCommand.ExecutionCount);
        Assert.AreEqual(null, _telegramBotClientWrapper.SentMessage);
    }
}

file abstract class TestChatCommand : ChatCommand
{
    public override string Name { get; protected set; }

    public int ExecutionCount { get; protected set; }

    public override Task ExecuteAsync(Message message, ChatState chatState, string rawParameters, ITelegramBotClientWrapper _telegramBotClientWrapper, CancellationToken cancellationToken)
    {
        ExecutionCount++;
        return Task.CompletedTask;
    }
}

file class TestGptCommand : TestChatCommand
{
    public override string Name { get => "gpt"; }
}
file class TestCommand : TestChatCommand
{
    public override string Name { get => "test"; }
}
