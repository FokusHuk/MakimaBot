using MakimaBot.Model;
using MakimaBot.Model.Processors;
using Moq;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class TestGptMessageProcessor
{
    private static Mock<IBucketClient> _bucketClientMock;
    private TestTelegramTextMessageSender _telegramTextMessageSender;
    private DataContext _dataContext;
    private CancellationToken _cancellationToken;
    private TestGptChatCommand _testCommand;
    private ChatCommandHandler _chatCommandHandler;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        var state = new TestBotStateBuilder()
                    .WithChat(new TestChatStateBuilder()
                        .WithId(1)
                        .WithName("TestChat")
                        .Build())
                    .Build();

        _bucketClientMock = new Mock<IBucketClient>();
        _bucketClientMock
            .Setup(x => x.LoadStateAsync())
            .ReturnsAsync(state);
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();

        _telegramTextMessageSender = new TestTelegramTextMessageSender();

        _dataContext = new DataContext(_bucketClientMock.Object);
        Task.Run(async () => _dataContext.ConfigureAsync()).GetAwaiter().GetResult();

        _testCommand = new TestGptChatCommand();
        _chatCommandHandler = new ChatCommandHandler(new[] { _testCommand });
    }

    private Message CreateTextMessage(string text)
    {
        var message = new Message();
        message.Text = text;
        return message;
    }

    [TestMethod]
    public async Task ReceiveMessageWith_RightRequest_SendRequestToApi()
    {
        var message = CreateTextMessage("@makima_daily_bot  gpt random   Promt  ");
        var gptMessageProcessor = new GptMessageProcessor(_dataContext, _chatCommandHandler, _telegramTextMessageSender);

        await gptMessageProcessor.ProcessChainAsync(message, 1, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is null);
        Assert.IsTrue(_testCommand.RawParameters is not null);
        Assert.IsTrue(_testCommand.MessageToGptApi is not null);
        Assert.AreEqual("random   Promt  ", _testCommand.RawParameters);
        Assert.AreEqual("@makima_daily_bot  gpt random   Promt  ", _testCommand.MessageToGptApi.Text);
    }

    [TestMethod]
    public async Task ReceiveMessageWith_WrongBotName_DoNothing()
    {
        var message = CreateTextMessage("@makiNO_daily_bot  gpt random   Promt  ");
        var gptMessageProcessor = new GptMessageProcessor(_dataContext, _chatCommandHandler, _telegramTextMessageSender);

        await gptMessageProcessor.ProcessChainAsync(message, 1, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is null);
        Assert.IsTrue(_testCommand.RawParameters is null);
        Assert.IsTrue(_testCommand.MessageToGptApi is null);
    }

    [TestMethod]
    public async Task ReceiveMessageWith_WrongGptCommand_SendErrorMessageToUser()
    {
        var message = CreateTextMessage("@makima_daily_bot  list of random   Promt  ");
        var gptMessageProcessor = new GptMessageProcessor(_dataContext, _chatCommandHandler, _telegramTextMessageSender);

        await gptMessageProcessor.ProcessChainAsync(message, 1, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is not null);
        Assert.AreEqual("Команда не распознана🙍‍♀️ запросите список доступных команд (list)", _telegramTextMessageSender.MessageSent.Text);
        Assert.IsTrue(_testCommand.RawParameters is null);
        Assert.IsTrue(_testCommand.MessageToGptApi is null);
    }

    [TestMethod]
    public async Task ReceiveMessageWith_EmptyPromt_SendErrorMessageToUser()
    {
        var message = CreateTextMessage("@makima_daily_bot gpt");
        var gptMessageProcessor = new GptMessageProcessor(_dataContext, _chatCommandHandler, _telegramTextMessageSender);

        await gptMessageProcessor.ProcessChainAsync(message, 1, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is not null);
        Assert.AreEqual("Кажется вы забыли указать что хотели узнать🤦‍♀️ @makima_daily_bot <gpt> <promt>", _telegramTextMessageSender.MessageSent.Text);
        Assert.IsTrue(_testCommand.RawParameters is null);
        Assert.IsTrue(_testCommand.MessageToGptApi is null);
    }

    [TestMethod]
    public async Task ReceiveMessageWith_OnlyBotName_SendErrorMessageToUser()
    {
        var message = CreateTextMessage("@makima_daily_bot");
        var gptMessageProcessor = new GptMessageProcessor(_dataContext, _chatCommandHandler, _telegramTextMessageSender);

        await gptMessageProcessor.ProcessChainAsync(message, 1, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is not null);
        Assert.AreEqual("Неизвестная команда💔 @makima_daily_bot <команда> <параметры>", _telegramTextMessageSender.MessageSent.Text);
        Assert.IsTrue(_testCommand.RawParameters is null);
        Assert.IsTrue(_testCommand.MessageToGptApi is null);
    }
}

public class TestGptChatCommand : ChatCommand
{
    public string RawParameters { get; set; }
    public Message MessageToGptApi { get; set; }

    public override string Name { get; set; } = "gpt";

    public override Task ExecuteAsync(Message message, 
                                      ChatState chatState, 
                                      string rawParameters, 
                                      ITelegramTextMessageSender _telegramTextMessageSender, 
                                      CancellationToken cancellationToken)
    {
        RawParameters = rawParameters;
        MessageToGptApi = new Message();
        MessageToGptApi.Text = message.Text;
        return Task.CompletedTask;
    }
}