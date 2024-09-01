using MakimaBot.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class GptMessageProcessor : ChatMessageProcessorBase
{
    private ChatCommandHandler _commandHandler;
    private TelegramBotClient _telegramBotClient;
    
    public GptMessageProcessor(DataContext dataContext, 
                               ChatCommandHandler commandHandler, 
                               TelegramBotClient telegramBotClient)
                               : base(dataContext)
    {
        _commandHandler = commandHandler;
        _telegramBotClient = telegramBotClient;
    }

    protected override async Task ProcessAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        var chatState = _dataContext.GetChatStateById(chatId);
        await _commandHandler.HandleAsync(message, chatState, _telegramBotClient, cancellationToken);
    }

    protected override bool ShouldLaunchAsync(Message message, long chatId, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(message.Text) 
               && message.Text.Trim().StartsWith("@makima_daily_bot");
    }
}
