using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MakimaBot.Model;

public class ChatCommandHandler : IChatCommandHandler
{
    private readonly IEnumerable<ChatCommand> _commands;

    public ChatCommandHandler(IEnumerable<ChatCommand> commands)
    {
        _commands = commands;
    }

    private const string CommandPattern = @"^@makima_daily_bot\s+([a-z]*)\s*(.*)$";
    private const string CommandError =
        $"""
        @makima\_daily\_bot это команда!
        Запросите список доступных команд ( `@makima_daily_bot list` )
        """;

    public async Task HandleAsync(
        Message message,
        ChatState chatState,
        ITelegramBotClientWrapper _telegramBotClientWrapper,
        CancellationToken cancellationToken)
    {
        var match = Regex.Matches(message.Text, CommandPattern, RegexOptions.IgnoreCase);

        if (match.Count == 0 || string.IsNullOrEmpty(match.First().Groups[1].Value))
        {
            await _telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                CommandError,
                replyToMessageId: message.MessageId,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }
        
        if(!chatState.UsersState.ContainsKey(message.From.Id))
        {
            await _telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                @"Вы не можете использовать команды, так как у вас еще нет роли.",
                replyToMessageId: message.MessageId,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var receivedCommandName = match.First().Groups[1].Value;
        var currentCommand = _commands.SingleOrDefault(command => command.Name == receivedCommandName);
        if (currentCommand is null)
        {
            await _telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                CommandError,
                replyToMessageId: message.MessageId,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var userState = chatState.UsersState[message.From.Id]; // ToDo: В dataContext вынести
        if (!userState.UserRole.AllowedCommands.Contains(currentCommand.Name))
        {
            await _telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                $"Вы не можете использовать эту команду, пока ваша роль: {userState.UserRole.RoleName}!",
                replyToMessageId: message.MessageId,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var rawParameters = match.First().Groups[2].Value;
        await currentCommand.ExecuteAsync(message, chatState, rawParameters, _telegramBotClientWrapper, cancellationToken); 
    }
}
