using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MakimaBot;

public class TelegramTextMessageSender : ITelegramTextMessageSender
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramTextMessageSender(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

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
        return _telegramBotClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            messageThreadId: messageThreadId,
            parseMode: parseMode,
            entities: entities,
            disableWebPagePreview: disableWebPagePreview,
            disableNotification: disableNotification,
            protectContent: protectContent,
            replyToMessageId: replyToMessageId,
            allowSendingWithoutReply: allowSendingWithoutReply,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }
}

