using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TestRandomPhraseProcessor : RandomPhraseProcessor
{
    public ProcessorsTestInfo testInfo;

    public TestRandomPhraseProcessor(IDataContext dataContext,
                                 ITelegramTextMessageSender telegramTextMessageSender,
                                 ProcessorsTestInfo testInfo)
                                 : base(dataContext, telegramTextMessageSender)
    {
        this.testInfo = testInfo;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        testInfo.ExecuteProcessor();
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return true;
    }
}
