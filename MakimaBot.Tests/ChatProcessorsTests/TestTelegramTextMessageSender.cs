using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MakimaBot.Tests;

public class TestTelegramTextMessageSender : ITelegramTextMessageSender
{
    public Message MessageSent { get; set; }

    public Task<Message> SendTextMessageAsync(ChatId chatId,
                                              string text, 
                                              int? messageThreadId = null, 
                                              ParseMode? parseMode = null, 
                                              IEnumerable<MessageEntity>? entities = null, 
                                              bool? disableWebPagePreview = null, 
                                              bool? disableNotification = null, 
                                              bool? protectContent = null, 
                                              int? replyToMessageId = null, 
                                              bool? allowSendingWithoutReply = null, 
                                              IReplyMarkup? replyMarkup = null, 
                                              CancellationToken cancellationToken = default)
    {
        MessageSent = new Message();
        MessageSent.Text = text;
        return Task<Message>.Run(() => MessageSent);
    }
}