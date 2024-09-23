using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TestHealthCheackProcessor : HealthCheackProcessor
{
    public ProcessorsTestInfo testInfo;
    public TestHealthCheackProcessor(IDataContext dataContext,
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
}
