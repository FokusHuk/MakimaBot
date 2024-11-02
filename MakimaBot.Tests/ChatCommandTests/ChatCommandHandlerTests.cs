using MakimaBot.Model;
using Telegram.Bot.Types;

namespace MakimaBot.Tests.ChatCommandHandlerTests;

[TestClass]
public class ChatCommandHandlerTests
{
    private TestTelegramBotClientWrapper _telegramBotClientWrapper;

    [TestInitialize]
    public void TestInitialize()
    {
        _telegramBotClientWrapper = new TestTelegramBotClientWrapper();
    }

    [TestMethod]
    [DataRow("@makima_daily_bot ")]
    [DataRow("@makima_daily_bot привет")]
    [DataRow("@makima_daily_bot гпт")]
    [DataRow("@makima_daily_bot")]
    [DataRow("@makima_daily_botcommand")]
    [DataRow("Makima @makima_daily_bot command parameters")]
    [DataRow("Макима @makima_daily_bot hello!")]
    public async Task HandleAsync_MentionOnly_SendUserError(string text)
    {
        var chatCommands = new List<TestChatCommand> { new TestFirstCommand(), new TestSecondCommand() };
        var chatCommandHandler = new ChatCommandHandler(chatCommands);
        var message = new Message().WithText(text);

        
        await chatCommandHandler.HandleAsync(
            message,
            new TestChatStateBuilder().Build(),
            _telegramBotClientWrapper, CancellationToken.None);

        
        Assert.IsTrue(chatCommands.All(command => command.ExecutionCount == 0));
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
        var chatCommands = new List<TestChatCommand> { new TestFirstCommand(), new TestSecondCommand() };
        var chatCommandHandler = new ChatCommandHandler(chatCommands);
        var message = new Message().WithText("@makima_daily_bot unknownCommand");

        
        await chatCommandHandler.HandleAsync(
            message,
            new TestChatStateBuilder().Build(),
             _telegramBotClientWrapper, CancellationToken.None);

        
        Assert.IsTrue(chatCommands.All(command => command.ExecutionCount == 0));
        Assert.AreEqual(
            """
            @makima\_daily\_bot это команда!
            Запросите список доступных команд ( `@makima_daily_bot list` )
            """,
            _telegramBotClientWrapper.SentMessage.Text);
    }

    [TestMethod]
    [DataRow("@makima_daily_bot secondCommand")]
    [DataRow("@makima_daily_bot secondCommand  ")]
    [DataRow("@makima_daily_bot    secondCommand")]
    [DataRow("@makima_daily_bot    secondCommand    ")]
    [DataRow("@makima_daily_bot secondCommand привет мир")]
    [DataRow("@makima_daily_bot secondCommand    привет мир")]
    public async Task HandleAsync_TestSecondCommand_ExecuteSecondCommand(string text)
    {
        var chatCommands = new List<TestChatCommand> { new TestFirstCommand(), new TestSecondCommand() };
        var chatCommandHandler = new ChatCommandHandler(chatCommands);
        var chatState = new TestChatStateBuilder()
            .WithUsers(new TestUserStatesBuilder()
                .WithUser(1, "", new string[] { "secondCommand" })
                .Build())
            .Build();
        var message = new Message()
            .WithText(text)
            .WithSender(1);

        
        await chatCommandHandler.HandleAsync(
            message,
            chatState,
            _telegramBotClientWrapper, CancellationToken.None);

        
        Assert.AreEqual(null, _telegramBotClientWrapper.SentMessage);
        CollectionAssert.AreEqual(
            new[] { 0, 1 },
            chatCommands.Select(command => command.ExecutionCount).ToArray());
    }

    [TestMethod]
    public async Task HandleAsync_FirstCommand_ExecuteFirstCommand()
    {
        var chatCommands = new List<TestChatCommand> { new TestFirstCommand(), new TestSecondCommand() };
        var chatCommandHandler = new ChatCommandHandler(chatCommands);
        var chatState = new TestChatStateBuilder()
            .WithUsers(new TestUserStatesBuilder()
                .WithUser(1, "", new string[] { "firstCommand" })
                .Build())
            .Build();
        var message = new Message()
            .WithText("@makima_daily_bot firstCommand parameters параметры")
            .WithSender(1);
        
        
        await chatCommandHandler.HandleAsync(
            message,
            chatState,
            _telegramBotClientWrapper, CancellationToken.None);

        
        Assert.AreEqual(null, _telegramBotClientWrapper.SentMessage);
        CollectionAssert.AreEqual(
            new[] { 1, 0 },
            chatCommands.Select(command => command.ExecutionCount).ToArray());
    }

    [TestMethod]
    [DataRow(2, "@makima_daily_bot firstCommand")]
    [DataRow(1, "@makima_daily_bot secondCommand")]
    public async Task HandleAsync_SecondCommandOrUnknownUser_AllowedFirstCommand_SendUserError(long userId, string expectedText)
    {
        var chatCommands = new List<TestChatCommand> { new TestFirstCommand(), new TestSecondCommand() };
        var chatCommandHandler = new ChatCommandHandler(chatCommands);
        var chatState = new TestChatStateBuilder()
            .WithUsers(new TestUserStatesBuilder()
                .WithUser(1, "", new string[] { "firstCommand" })
                .Build())
            .Build();
        var message = new Message()
            .WithText(expectedText)
            .WithSender(userId);
        
        
        await chatCommandHandler.HandleAsync(
            message,
            chatState,
            _telegramBotClientWrapper, CancellationToken.None);
        
        
        Assert.IsTrue(chatCommands.All(command => command.ExecutionCount == 0));
        Assert.AreEqual(
             """
             Доступ к команде запрещен!
             Запросите список доступных команд ( `@makima_daily_bot list` )
             """,
            _telegramBotClientWrapper.SentMessage.Text);
    }
}

file abstract class TestChatCommand : ChatCommand
{
    public override string Name { get; protected set; }

    public int ExecutionCount { get; protected set; }

    public override Task ExecuteAsync(
        Message message, 
        ChatState chatState, 
        string rawParameters, 
        ITelegramBotClientWrapper _telegramBotClientWrapper, 
        CancellationToken cancellationToken)
    {
        ExecutionCount++;
        return Task.CompletedTask;
    }
}

file class TestFirstCommand : TestChatCommand
{
    public override string Name { get => "firstCommand"; }
}
file class TestSecondCommand : TestChatCommand
{
    public override string Name { get => "secondCommand"; }
}
