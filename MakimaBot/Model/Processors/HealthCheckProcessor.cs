using MakimaBot.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class HealthCheackProcessor : ProcessorBase
{
    private TelegramBotClient _telegramBotClient;

    public HealthCheackProcessor(DataContext dataContext, 
                                 TelegramBotClient telegramBotClient) 
                                 : base(dataContext)
    {
        _telegramBotClient = telegramBotClient;
    }

    protected override async Task ExecuteBody(Message message, long chatId, CancellationToken cancellationToken)
    {
        if (message.Sticker.Emoji == "😤")
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatId: chatId,
                text: "❤️",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
        }
    }

    protected override bool ExecuteCondition(Message message, long chatId, CancellationToken cancellationToken)
    {
        return message.Sticker is { SetName: { } } sticker &&
            sticker.SetName.Equals("makimapak", StringComparison.InvariantCultureIgnoreCase);
    }
}
