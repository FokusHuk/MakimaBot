using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class TestUntrustedChatProcessor : UntrustedChatProcessor
{
    public ProcessorsTestInfo testInfo;

    public TestUntrustedChatProcessor(IDataContext dataContext, ProcessorsTestInfo testInfo) : base(dataContext)
    {
        this.testInfo = testInfo;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
       testInfo.ExecuteProcessor();
    }
}
