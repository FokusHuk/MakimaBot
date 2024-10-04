using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MakimaBot.Tests;

public class TestTelegramBotClientWrapper : ITelegramBotClientWrapper
{
    public Message SentMessage { get; set; }

    public Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ChatMember> GetChatMemberAsync(ChatId chatId, long userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetMeAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Update[]> GetUpdatesAsync(int? offset = null, int? limit = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Message> SendTextMessageAsync(ChatId chatId,
                                              string text, 
                                              ParseMode? parseMode = null, 
                                              int? replyToMessageId = null,  
                                              CancellationToken cancellationToken = default)
    {
        SentMessage = new Message();
        SentMessage.Text = text;
        return Task.FromResult(SentMessage);
    }
}
