using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MakimaBot;

public class TelegramBotClientWrapper : ITelegramBotClientWrapper
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramBotClientWrapper(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = default)
    {
        return _telegramBotClient.GetChatAsync(chatId, cancellationToken);
    }

    public Task<ChatMember> GetChatMemberAsync(ChatId chatId, long userId, CancellationToken cancellationToken = default)
    {
        return _telegramBotClient.GetChatMemberAsync(chatId, userId, cancellationToken);
    }

    public Task<User> GetMeAsync(CancellationToken cancellationToken = default)
    {
        return _telegramBotClient.GetMeAsync(cancellationToken);
    }

    public Task<Update[]> GetUpdatesAsync(int? offset = null, int? limit = null, CancellationToken cancellationToken = default)
    {
        return _telegramBotClient.GetUpdatesAsync(offset, limit, cancellationToken: cancellationToken);
    }

    public Task<Message> SendTextMessageAsync(ChatId chatId,
                                              string text,
                                              ParseMode? parseMode = null,
                                              int? replyToMessageId = null,
                                              CancellationToken cancellationToken = default)
    {
        return _telegramBotClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            parseMode: parseMode,
            replyToMessageId: replyToMessageId,
            cancellationToken: cancellationToken);
    }
}
