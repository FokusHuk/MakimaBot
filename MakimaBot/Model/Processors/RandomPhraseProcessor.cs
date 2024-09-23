using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class RandomPhraseProcessor : ChatMessageProcessorBase
{
    private ITelegramTextMessageSender _telegramTextMessageSender;

    public RandomPhraseProcessor(IDataContext dataContext,
                                 ITelegramTextMessageSender telegramTextMessageSender)
                                 : base(dataContext)
    {
        _telegramTextMessageSender = telegramTextMessageSender;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        
        var reactions = new[]
            {
                "Я все вижу 👀",
                "Хватит сюда писать",
                "...",
                "Не надо слов, просто погавкай 🐶",
                "Я сейчас ливну отсюда",
                "Ахахаххахахаха",
                "До вечера 🌙"
            };

        await _telegramTextMessageSender.SendTextMessageAsync(
            chatId: chatState.ChatId,
            text: reactions[new Random().Next(reactions.Length)],
            cancellationToken: cancellationToken);
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken) => new Random().Next(10) < 1;
}
