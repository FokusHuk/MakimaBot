namespace MakimaBot.Model.Processors;

public class ProcessorComponent
{
    public ProcessorComponent(DailyActivityProcessor dailyActivityProcessor, 
                              GptMessageProcessor gptMessageProcessor, 
                              HealthCheackProcessor healthCheackProcessor, 
                              RandomPhraseProcessor randomPhraseProcessor, 
                              TrustedChatProcessor trustedChatProcessor, 
                              UntrustedChatProcessor untrustedChatProcessor)
    {
        DailyActivityProcessor = dailyActivityProcessor;
        GptMessageProcessor = gptMessageProcessor;
        HealthCheackProcessor = healthCheackProcessor;
        RandomPhraseProcessor = randomPhraseProcessor;
        TrustedChatProcessor = trustedChatProcessor;
        UntrustedChatProcessor = untrustedChatProcessor;
    }

    public DailyActivityProcessor DailyActivityProcessor {get; }
    public GptMessageProcessor GptMessageProcessor { get; }
    public HealthCheackProcessor HealthCheackProcessor { get; }
    public RandomPhraseProcessor RandomPhraseProcessor { get; }
    public TrustedChatProcessor TrustedChatProcessor { get; }
    public UntrustedChatProcessor UntrustedChatProcessor { get; }
}