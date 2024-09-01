namespace MakimaBot.Model.Processors;

public abstract class ProcessorsChainFactory
{
    protected readonly ProcessorComponent _component;

    protected ProcessorsChainFactory(ProcessorComponent component)
    {
        _component = component;
    }

    public abstract ChatMessageProcessorBase CreateChain();
}