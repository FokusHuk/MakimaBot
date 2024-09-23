using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TestGptMessageProcessor : GptMessageProcessor
{
    public ProcessorsTestInfo testInfo;
    public TestGptMessageProcessor(IDataContext dataContext,
                               IChatCommandHandler commandHandler,
                               ITelegramTextMessageSender telegramTextMessageSender,
                               ProcessorsTestInfo testInfo)
                               : base(dataContext, commandHandler, telegramTextMessageSender)
    {
        this.testInfo = testInfo;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        testInfo.ExecuteProcessor();
    }
}
