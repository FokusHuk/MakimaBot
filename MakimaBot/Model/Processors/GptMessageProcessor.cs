using MakimaBot.Model;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Processors;

public class GptMessageProcessor : ProcessorBase
{
    private ChatCommandHandler _commandHandler;
    private TelegramBotClient _telegramBotClient;
    
    public GptMessageProcessor(ChatCommandHandler commandHandler, TelegramBotClient telegramBotClient)
    {
        _commandHandler = commandHandler;
        _telegramBotClient = telegramBotClient;
    }

    protected override async Task ExecuteBody(Message message, ChatState chatState, CancellationToken cancellationToken)
    {
        await _commandHandler.HandleAsync(message, chatState, _telegramBotClient, cancellationToken);
    }

    protected override bool ExecuteCondition(Message message, ChatState chatState)
    {
        return !string.IsNullOrWhiteSpace(message.Text) 
               && message.Text.Trim().StartsWith("@makima_daily_bot");
    }
}
