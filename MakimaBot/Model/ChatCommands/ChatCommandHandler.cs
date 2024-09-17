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
        ITelegramTextMessageSender _telegramTextMessageSender,
        CancellationToken cancellationToken)
    {
        var match = Regex.Matches(message.Text, CommandPattern, RegexOptions.IgnoreCase);

        var commandName = match.First().Groups[1].Value;
        if (string.IsNullOrEmpty(commandName))
        {
            await _telegramTextMessageSender.SendTextMessageAsync(
                chatState.ChatId,
                "Неизвестная команда💔 @makima_daily_bot <команда> <параметры>", // todo: сообщить юзеру, что меншн это команда
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        var allowedCommands = _commands.Select(command => command.Name).ToArray(); // todo: что? зачем сейчас? почему? Кажется тут чего-то не хватает, например премиссий💔

        var currentCommand = _commands.SingleOrDefault(command => command.Name == commandName);
        if (currentCommand is null)
        {
            await _telegramTextMessageSender.SendTextMessageAsync(
                chatState.ChatId,
                "Команда не распознана🙍‍♀️ запросите список доступных команд (list)",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        var rawParameters = match.First().Groups[2].Value;
        if (string.IsNullOrEmpty(rawParameters))
        {
            await _telegramTextMessageSender.SendTextMessageAsync(
                chatState.ChatId,
                "Кажется вы забыли указать что хотели узнать🤦‍♀️ @makima_daily_bot <gpt> <promt>",
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        await currentCommand.ExecuteAsync(message, chatState, rawParameters, _telegramTextMessageSender, cancellationToken); 
    }
}
