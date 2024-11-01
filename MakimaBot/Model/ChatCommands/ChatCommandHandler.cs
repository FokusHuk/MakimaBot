using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MakimaBot.Model;

public class ChatCommandHandler(IEnumerable<ChatCommand> commands) : IChatCommandHandler
{
    private const string CommandPattern = @"^@makima_daily_bot\s+([a-z]*)\s*(.*)$";
    private const string CommandError =
        $"""
        @makima\_daily\_bot это команда!
        Запросите список доступных команд ( `@makima_daily_bot list` )
        """;

    public async Task HandleAsync(
        Message message,
        ChatState chatState,
        ITelegramBotClientWrapper telegramBotClientWrapper,
        CancellationToken cancellationToken)
    {
        var match = Regex.Matches(message.Text, CommandPattern, RegexOptions.IgnoreCase);

        if (match.Count == 0 || string.IsNullOrEmpty(match.First().Groups[1].Value))
        {
            await telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                CommandError,
                replyToMessageId: message.MessageId,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var receivedCommandName = match.First().Groups[1].Value;
        var currentCommand = commands.SingleOrDefault(command => command.Name == receivedCommandName);
        if (currentCommand is null)
        {
            await telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                CommandError,
                replyToMessageId: message.MessageId,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var knownUser = chatState.Users.FirstOrDefault(user => user.UserId == message.From.Id);
        if (knownUser is null || !knownUser.AllowedCommands.Contains(currentCommand.Name))
        {
            await telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                $"""
                 Доступ к команде запрещен!
                 Запросите список доступных команд ( `@makima_daily_bot list` )
                 """,
                replyToMessageId: message.MessageId,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var rawParameters = match.First().Groups[2].Value;
        await currentCommand.ExecuteAsync(message, chatState, rawParameters, telegramBotClientWrapper, cancellationToken); 
    }
}
