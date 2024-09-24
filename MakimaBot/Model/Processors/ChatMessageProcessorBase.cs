using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public abstract class ChatMessageProcessorBase
{
    private ChatMessageProcessorBase _root;
    private ChatMessageProcessorBase _processor;
    private ChatMessageProcessorBase _childProcessor;

    protected readonly IDataContext _dataContext;

    protected ChatMessageProcessorBase(IDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public virtual bool СontinueAnyway { get => false; }

    public ChatMessageProcessorBase ChainedWith(ChatMessageProcessorBase processor)
    {
        if(_root is null)
        {
            _root = this;      
        }

        _processor = processor;
        _processor._root = this._root;
        return _processor;
    }

    public ChatMessageProcessorBase SubchainedWith(ChatMessageProcessorBase childProcessor)
    {
        _childProcessor = childProcessor;
        return this;
    }

    public ChatMessageProcessorBase EndChainWith(ChatMessageProcessorBase processor)
    {
        if (_root is null)
        {
            _root = this;
        }

        _processor = processor;
        _processor._root = this._root;
        return _root;
    }
    
    public async Task ProcessChainAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        if (ShouldLaunchAsync(message, chatId, cancellationToken))
        {
            await ProcessAsync(message, chatId, cancellationToken);

            if (_childProcessor != null)
            {
                await _childProcessor.ProcessChainAsync(message, chatId, cancellationToken);
            }

            if (СontinueAnyway && _processor != null)
            {
                await _processor.ProcessChainAsync(message, chatId, cancellationToken);
            }
        }
        else if (_processor != null)
        {
            await _processor.ProcessChainAsync(message, chatId, cancellationToken);
        }
    }

    protected abstract Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken);

    protected abstract bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken);
}
