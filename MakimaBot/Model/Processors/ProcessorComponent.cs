namespace MakimaBot.Model.Processors;

public class ProcessorComponent
{
    private readonly DailyActivityProcessor _dailyActivityProcessor;
    private readonly GptMessageProcessor _gptMessageProcessor;
    private readonly HealthCheackProcessor _healthCheackProcessor;
    private readonly RandomPhraseProcessor _randomPhraseProcessor;
    private readonly TrustedChatProcessor _trustedChatProcessor;
    private readonly UntrustedChatProcessor _untrustedChatProcessor;

    public ProcessorComponent(DailyActivityProcessor dailyActivityProcessor, 
                              GptMessageProcessor gptMessageProcessor, 
                              HealthCheackProcessor healthCheackProcessor, 
                              RandomPhraseProcessor randomPhraseProcessor, 
                              TrustedChatProcessor trustedChatProcessor, 
                              UntrustedChatProcessor untrustedChatProcessor)
    {
        _dailyActivityProcessor = dailyActivityProcessor;
        _gptMessageProcessor = gptMessageProcessor;
        _healthCheackProcessor = healthCheackProcessor;
        _randomPhraseProcessor = randomPhraseProcessor;
        _trustedChatProcessor = trustedChatProcessor;
        _untrustedChatProcessor = untrustedChatProcessor;
    }

    public ProcessorBase GetProcessor()
    {
        return _healthCheackProcessor
        .ChainedWith(_untrustedChatProcessor)
        .EndChainWith(_trustedChatProcessor
            .SubchainedWith(_gptMessageProcessor
                .ChainedWith(_dailyActivityProcessor)
                .EndChainWith(_randomPhraseProcessor)));
    }
}