using MakimaBot.Model.Processors;
using Telegram.Bot.Types;

namespace MakimaBot.Tests;

[TestClass]
public class TestHealthCheackProcessor
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

    private Message CreateMessageWithSticker(string stickerSetName, string emoji)
    {
        var message = new Message();
        message.Sticker = new Sticker();
        message.Sticker.SetName = stickerSetName;
        message.Sticker.Emoji = emoji;
        return message;
    }

    [TestMethod]
    public async Task CheckHealthWithSticker_RightSticker_SendHeartrBack()
    {
        var message = CreateMessageWithSticker("makimapak", "😤");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
        var expectedMessageText = "❤️";

        await healthCheckProcessor.ProcessChainAsync(message, _chatId, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is not null);
        Assert.AreEqual(expectedMessageText, _telegramTextMessageSender.MessageSent.Text);
    }

    [TestMethod]
    public async Task CheckHealthWithSticker_WrongStickerSetName_DoNothing()
    {
        var message = CreateMessageWithSticker("randomName", "😤");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
        
        await healthCheckProcessor.ProcessChainAsync(message, _chatId, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is null);
    }

    [TestMethod]
    public async Task CheckHealthWithSticker_WrongSticker_DoNothing()
    {
        var message = CreateMessageWithSticker("makimapak", "❤️");
        var healthCheckProcessor = new HealthCheackProcessor(dataContext: null, _telegramTextMessageSender);
 
        await healthCheckProcessor.ProcessChainAsync(message, _chatId, _cancellationToken);

        Assert.IsTrue(_telegramTextMessageSender.MessageSent is null);
    }
}
