using MakimaBot.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class UntrustedChatProcessor : ProcessorBase
{
    private readonly DataContext _dataContext;
    private readonly TelegramBotClient _telegramBotClient;

    private readonly long _chatId;

    public UntrustedChatProcessor(DataContext dataContext, TelegramBotClient telegramBotClient, long chatId)
    {
        _dataContext = dataContext;
        _telegramBotClient = telegramBotClient;
        _chatId = chatId;
    }

    protected override async Task ExecuteBody(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendTextMessageAsync(
               chatId: _chatId,
               text: "Привет! Я Макима.\nИ мне запрещают общаться с незнакомцами. Но если очень хочется, можете написать хозяину :)",//\nhttps://t.me/akima_yooukie",
               cancellationToken: cancellationToken);

        _dataContext.AddUnknownMessage(
            sentDateTimeUtc: DateTime.UtcNow,
            chatId: _chatId,
            message: message.Text,
            username: message.From?.Username ?? message.From?.FirstName ?? message.From?.LastName ?? message.From?.Id.ToString());

        await _dataContext.SaveChangesAsync();
    }

    protected override bool ExecuteCondition(Message message, ChatState chatState) => chatState is null;
}
