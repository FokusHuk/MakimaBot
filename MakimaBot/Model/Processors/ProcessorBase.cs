using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public abstract class ProcessorBase
{
    protected ProcessorBase _root;
    protected ProcessorBase _processor;
    protected ProcessorBase _childProcessor;

    protected readonly DataContext _dataContext;

    protected ProcessorBase(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

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
    
    public async Task Execute(Message message, long chatId, CancellationToken cancellationToken)
    {
        if (ExecuteCondition(message, chatId, cancellationToken))
        {
            await ExecuteBody(message, chatId, cancellationToken);

            if (_childProcessor != null)
            {
                await _childProcessor.Execute(message, chatId, cancellationToken);
            }

            if (_continueAnyway && _processor != null)
            {
                await _processor.Execute(message, chatId, cancellationToken);
            }
        }
        else if (_processor != null)
        {
            await _processor.Execute(message, chatId, cancellationToken);
        }
    }

    protected abstract Task ExecuteBody(Message message, long chatId, CancellationToken cancellationToken);

    protected abstract bool ExecuteCondition(Message message, long chatId, CancellationToken cancellationToken);
}
