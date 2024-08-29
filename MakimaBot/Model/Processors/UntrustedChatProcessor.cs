using MakimaBot.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class UntrustedChatProcessor : ProcessorBase
{
    private readonly TelegramBotClient _telegramBotClient;

    public UntrustedChatProcessor(DataContext dataContext, 
                                  TelegramBotClient telegramBotClient) 
                                  : base(dataContext)
    {
        _telegramBotClient = telegramBotClient;
    }

    protected override async Task ExecuteBody(Message message, long chatId, CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendTextMessageAsync(
               chatId: chatId,
               text: "Привет! Я Макима.\nИ мне запрещают общаться с незнакомцами. Но если очень хочется, можете написать хозяину :)",//\nhttps://t.me/akima_yooukie",
               cancellationToken: cancellationToken);

        _dataContext.AddUnknownMessage(
            sentDateTimeUtc: DateTime.UtcNow,
            chatId: chatId,
            message: message.Text,
            username: message.From?.Username ?? message.From?.FirstName ?? message.From?.LastName ?? message.From?.Id.ToString());

        await _dataContext.SaveChangesAsync();
    }

    protected override bool ExecuteCondition(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        return chatState is null;
    }
}
