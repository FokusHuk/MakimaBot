using MakimaBot.Model.Processors;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class HealthCkeckProcessorTests
{
    private static long _chatId = 1;
    private CancellationToken _cancellationToken;
    private TestTelegramTextMessageSender _telegramTextMessageSender;

    [TestInitialize]
    public void TestInitialize()
    {
        _cancellationToken = new CancellationToken();
        _telegramTextMessageSender = new TestTelegramTextMessageSender();
    }

    [TestMethod]
    public async Task ReceiveSticker_RightSticker_SendHeartrBack()
    {
        var message = new Message().AddSticker(setName: "makimapak", emoji: "😤");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
        var expectedMessageText = "❤️";

        await healthCheckProcessor.ProcessChainAsync(message, _chatId, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is not null);
        Assert.AreEqual(expectedMessageText, _telegramTextMessageSender.MessageSent.Text);
    }

    [TestMethod]
    public async Task ReceiveSticker_WrongStickerSetName_DoNothing()
    {
        var message = new Message().AddSticker(setName: "randomName", emoji: "😤");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
        
        await healthCheckProcessor.ProcessChainAsync(message, _chatId, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is null);
    }

    [TestMethod]
    public async Task ReceiveSticker_WrongSticker_DoNothing()
    {
        var message = new Message().AddSticker(setName: "makimapak", emoji: "❤️");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
 
        await healthCheckProcessor.ProcessChainAsync(message, _chatId, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is null);
    }
}
