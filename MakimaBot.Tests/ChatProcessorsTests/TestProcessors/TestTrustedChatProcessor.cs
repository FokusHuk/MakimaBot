using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TestTrustedChatProcessor : TrustedChatProcessor
{
    public ProcessorsTestInfo testInfo;

    public TestTrustedChatProcessor(IDataContext dataContext, ProcessorsTestInfo testInfo) : base(dataContext)
    {
        this.testInfo = testInfo;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken) // async
    {
        testInfo.ExecuteProcessor();
    }
}
