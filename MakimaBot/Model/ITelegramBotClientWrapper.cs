using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MakimaBot;

public interface ITelegramBotClientWrapper
{
    Task<Message> SendTextMessageAsync(ChatId chatId,
                                       string text,
                                       ParseMode? parseMode = null,
                                       int? replyToMessageId = null,
                                       CancellationToken cancellationToken = default);

    Task<Update[]> GetUpdatesAsync(int? offset = null,
                                 int? limit = null,
                                 CancellationToken cancellationToken = default);

    int GetHashCode();

    Task<User> GetMeAsync(CancellationToken cancellationToken = default);

    Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = default);

    Task<ChatMember> GetChatMemberAsync(ChatId chatId, long userId, CancellationToken cancellationToken = default);  

    Task SendDocumentAsync(ChatId chatId, InputFile file);
}
