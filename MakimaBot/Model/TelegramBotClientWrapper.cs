using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MakimaBot;

public class TelegramBotClientWrapper(
    ITelegramBotClient telegramBotClient) : ITelegramBotClientWrapper
{
    public async Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = default) =>
        await telegramBotClient.GetChatAsync(chatId, cancellationToken);

    public async Task<ChatMember> GetChatMemberAsync(ChatId chatId, long userId, CancellationToken cancellationToken = default) =>
        await telegramBotClient.GetChatMemberAsync(chatId, userId, cancellationToken);
    
    public async Task<User> GetMeAsync(CancellationToken cancellationToken = default) => 
        await telegramBotClient.GetMeAsync(cancellationToken);
    
    public async Task<Update[]> GetUpdatesAsync(int? offset = null, int? limit = null, CancellationToken cancellationToken = default) =>
        await telegramBotClient.GetUpdatesAsync(offset, limit, cancellationToken: cancellationToken);

    public async Task<Message> SendTextMessageAsync(ChatId chatId,
                                              string text,
                                              ParseMode? parseMode = null,
                                              int? replyToMessageId = null,
                                              CancellationToken cancellationToken = default)
    {
        return await telegramBotClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            parseMode: parseMode,
            replyToMessageId: replyToMessageId,
            cancellationToken: cancellationToken);
    }

    public async Task SendDocumentAsync(ChatId chatId, InputFile file) =>
        await telegramBotClient.SendDocumentAsync(chatId, file);
}
