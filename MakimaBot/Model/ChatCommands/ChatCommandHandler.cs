using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class ChatCommandHandler : IChatCommandHandler
{
    private readonly IEnumerable<ChatCommand> _commands;

    public ChatCommandHandler(IEnumerable<ChatCommand> commands)
    {
        _commands = commands;
    }

    private const string CommandPattern = @"^@makima_daily_bot\s*([a-z]*)\s*(.*)$";

    public async Task HandleAsync(
        Message message,
        ChatState chatState,
        ITelegramTextMessageSender _telegramTextMessageSender,
        CancellationToken cancellationToken)
    {
        var match = Regex.Matches(message.Text, CommandPattern, RegexOptions.IgnoreCase);

        var commandName = match.First().Groups[1].Value;
        if (string.IsNullOrEmpty(commandName))
        {
            await _telegramTextMessageSender.SendTextMessageAsync(
                chatState.ChatId,
                @"@makima\_daily\_bot это команда! Запросите список доступных команд ( `@makima_daily_bot list` )",
                replyToMessageId: message.MessageId,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var currentCommand = _commands.SingleOrDefault(command => command.Name == commandName);
        if (currentCommand is null)
        {
            await _telegramTextMessageSender.SendTextMessageAsync(
                chatState.ChatId,
                $"Команда < **{commandName.Trim()}** >  не распознана🙍‍♀️ Запросите список доступных команд ( `@makima_daily_bot list` )",
                replyToMessageId: message.MessageId,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var rawParameters = match.First().Groups[2].Value;

        await currentCommand.ExecuteAsync(message, chatState, rawParameters, _telegramTextMessageSender, cancellationToken); 
    }
}
