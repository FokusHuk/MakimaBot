using MakimaBot.Model.Processors;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class HealthCkeckProcessorTests
{
    private const long ExistedChatId = 1;
    private TestTelegramBotClientWrapper _telegramBotClientWrapper;

    [TestInitialize]
    public void TestInitialize()
    {
        _telegramBotClientWrapper = new TestTelegramBotClientWrapper();
    }

    [TestMethod]
    public async Task ProcessChainAsync_RightSticker_SendHeartrBack()
    {
        var message = new Message().WithSticker(setName: "makimapak", emoji: "😤");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramBotClientWrapper);
        var expectedMessageText = "❤️";

        await healthCheckProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        Assert.IsTrue(_telegramBotClientWrapper.SentMessage is not null);
        Assert.AreEqual(expectedMessageText, _telegramBotClientWrapper.SentMessage.Text);
    }

    [TestMethod]
    public async Task ProcessChainAsync_WrongStickerSetName_DoNothing()
    {
        var message = new Message().WithSticker(setName: "randomName", emoji: "😤");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramBotClientWrapper);
        
        await healthCheckProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        Assert.IsTrue(_telegramBotClientWrapper.SentMessage is null);
    }

    [TestMethod]
    public async Task ProcessChainAsync_WrongSticker_DoNothing()
    {
        var message = new Message().WithSticker(setName: "makimapak", emoji: "❤️");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramBotClientWrapper);
 
        await healthCheckProcessor.ProcessChainAsync(message, ExistedChatId, CancellationToken.None);

        Assert.IsTrue(_telegramBotClientWrapper.SentMessage is null);
    }
}
