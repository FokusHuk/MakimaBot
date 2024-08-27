using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class RandomPhraseProcessor : ProcessorBase
{
    private TelegramBotClient _telegramBotClient;

    public RandomPhraseProcessor(TelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    protected override  async Task ExecuteBody(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
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

        await _telegramBotClient.SendTextMessageAsync(
            chatState.ChatId,
            reactions[new Random().Next(reactions.Length)],
            cancellationToken: cancellationToken);
    }

    protected override bool ExecuteCondition(Message message, ChatState chatState) => new Random().Next(10) < 1;
}
