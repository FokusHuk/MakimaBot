namespace MakimaBot.Model.Processors;

public class ProcessorComponent
{
    public ProcessorComponent(
        DailyActivityProcessor dailyActivityProcessor, 
        ChatCommandProcessor chatCommandProcessor, 
        HealthCheackProcessor healthCheackProcessor, 
        RandomPhraseProcessor randomPhraseProcessor, 
        TrustedChatProcessor trustedChatProcessor, 
        UntrustedChatProcessor untrustedChatProcessor)
    {
        DailyActivityProcessor = dailyActivityProcessor;
        ChatCommandProcessor = chatCommandProcessor;
        HealthCheackProcessor = healthCheackProcessor;
        RandomPhraseProcessor = randomPhraseProcessor;
        TrustedChatProcessor = trustedChatProcessor;
        UntrustedChatProcessor = untrustedChatProcessor;
    }

    public DailyActivityProcessor DailyActivityProcessor { get; }
    public ChatCommandProcessor ChatCommandProcessor { get; }
    public HealthCheackProcessor HealthCheackProcessor { get; }
    public RandomPhraseProcessor RandomPhraseProcessor { get; }
    public TrustedChatProcessor TrustedChatProcessor { get; }
    public UntrustedChatProcessor UntrustedChatProcessor { get; }
}