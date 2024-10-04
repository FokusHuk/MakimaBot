using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class RandomPhraseProcessor : ChatMessageProcessorBase
{
    private ITelegramBotClientWrapper _telegramBotClientWrapper;

    public RandomPhraseProcessor(
        IDataContext dataContext,
        ITelegramBotClientWrapper telegramBotClientWrapper)
        : base(dataContext)
    {
        _telegramBotClientWrapper = telegramBotClientWrapper;
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

        await _telegramBotClientWrapper.SendTextMessageAsync(
            chatId: chatState.ChatId,
            text: reactions[new Random().Next(reactions.Length)],
            cancellationToken: cancellationToken);
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken) => new Random().Next(10) < 1;
}
