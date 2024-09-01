namespace MakimaBot.Model.Processors;

public class DefaultProcessorsChainFactory : ProcessorsChainFactory
{
    public DefaultProcessorsChainFactory(ProcessorComponent component) : base(component)
    {
    }

    public override ChatMessageProcessorBase CreateChain()
    {
        return _component.HealthCheackProcessor
        .ChainedWith(_component.UntrustedChatProcessor)
        .EndChainWith(_component.TrustedChatProcessor
            .SubchainedWith(_component.GptMessageProcessor
                .ChainedWith(_component.DailyActivityProcessor)
                .EndChainWith(_component.RandomPhraseProcessor)
            )
        );
    }
}
