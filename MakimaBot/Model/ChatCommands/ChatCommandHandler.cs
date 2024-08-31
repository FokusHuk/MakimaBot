using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class ChatCommandHandler
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
        TelegramBotClient _telegramBotClient,
        CancellationToken cancellationToken)
    {
        var match = Regex.Matches(message.Text, CommandPattern, RegexOptions.IgnoreCase);

        if (match.Count == 0)
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatState.ChatId,
                "Неизвестная команда", // todo: сообщить юзеру, что меншн это команда
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        var commandName = match.First().Groups[1].Value;

        var allowedCommands = _commands.Select(command => command.Name).ToArray();

        var currentCommand = _commands.SingleOrDefault(command => command.Name == commandName);

        if (currentCommand == null)
        {
            await _telegramBotClient.SendTextMessageAsync(
                chatState.ChatId,
                "Команда не распознана, запросите список доступных команд (list)",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        var rawParameters = match.First().Groups[2].Value;

        await currentCommand.ExecuteAsync(message, chatState, rawParameters, _telegramBotClient, cancellationToken); 
    }
}
