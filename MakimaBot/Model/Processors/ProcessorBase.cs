using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public abstract class ProcessorBase
{
    protected ProcessorBase _root;
    protected ProcessorBase _processor;
    protected ProcessorBase _childProcessor;

    protected virtual bool _continueAnyway => false;

    public ProcessorBase ChainedWith(ProcessorBase processor)
    {
        if(_root is null)
        {
            _root = this;      
        }

        _processor = processor;
        _processor._root = this._root;
        return _processor;
    }

    public ProcessorBase SubchainedWith(ProcessorBase childProcessor)
    {
        _childProcessor = childProcessor;
        return this;
    }


    public ProcessorBase EndChainWith(ProcessorBase processor)
    {
        if (_root is null)
        {
            _root = this;
        }

        _processor = processor;
        _processor._root = this._root;
        return _root;
    }
    
    public async Task Execute(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
        if (ExecuteCondition(message, chatState))
        {
            await ExecuteBody(message, chatState, cancellationToken);

            if (_childProcessor != null)
            {
                await _childProcessor.Execute(message, chatState, cancellationToken);
            }

            if (_continueAnyway && _processor != null)
            {
                await _processor.Execute(message, chatState, cancellationToken);
            }
        }
        else if (_processor != null)
        {
            await _processor.Execute(message, chatState, cancellationToken);
        }

    }

    protected abstract Task ExecuteBody(Message message, ChatState chatState, CancellationToken cancellationToken);

    protected abstract bool ExecuteCondition(Message message, ChatState chatState);
}
