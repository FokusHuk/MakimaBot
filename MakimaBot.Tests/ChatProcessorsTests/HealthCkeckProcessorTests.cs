using MakimaBot.Model.Processors;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class HealthCkeckProcessorTests
{
    private const long ExistedChatId = 1;
    private TestTelegramTextMessageSender _telegramTextMessageSender;

    [TestInitialize]
    public void TestInitialize()
    {
        _telegramTextMessageSender = new TestTelegramTextMessageSender();
    }

    [TestMethod]
    public async Task ProcessChainAsync_RightSticker_SendHeartrBack()
    {
        var message = new Message().WithSticker(setName: "makimapak", emoji: "😤");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
        var expectedMessageText = "❤️";

        await healthCheckProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        Assert.IsTrue(_telegramTextMessageSender.SentMessage is not null);
        Assert.AreEqual(expectedMessageText, _telegramTextMessageSender.SentMessage.Text);
    }

    [TestMethod]
    public async Task ProcessChainAsync_WrongStickerSetName_DoNothing()
    {
        var message = new Message().WithSticker(setName: "randomName", emoji: "😤");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
        
        await healthCheckProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        Assert.IsTrue(_telegramTextMessageSender.SentMessage is null);
    }

    [TestMethod]
    public async Task ProcessChainAsync_WrongSticker_DoNothing()
    {
        var message = new Message().WithSticker(setName: "makimapak", emoji: "❤️");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
 
        await healthCheckProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        Assert.IsTrue(_telegramTextMessageSender.SentMessage is null);
    }
}
