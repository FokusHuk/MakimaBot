using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class ChatCommandProcessor : ChatMessageProcessorBase
{
    private IChatCommandHandler _commandHandler;
    private ITelegramBotClientWrapper _telegramBotClientWrapper;

    public ChatCommandProcessor(
        IDataContext dataContext,
        IChatCommandHandler commandHandler,
        ITelegramBotClientWrapper telegramBotClientWrapper)
        : base(dataContext)
    {
        _commandHandler = commandHandler;
        _telegramBotClientWrapper = telegramBotClientWrapper;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        await _commandHandler.HandleAsync(message, chatState, _telegramBotClientWrapper, cancellationToken);
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(message.Text)
               && message.Text.Trim().StartsWith("@makima_daily_bot")
               && message.ForwardDate is null;
    }
}
