using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace MakimaBot.Model;

public class ChatCommandHandler : IChatCommandHandler
{
    private readonly IEnumerable<ChatCommand> _commands;

    public ChatCommandHandler(IEnumerable<ChatCommand> commands)
    {
        _commands = commands;
    }

    private const string CommandPattern = @"^@makima_daily_bot\s+([a-z]*)\s*(.*)$";

    private string GetGptChatError(string commandError = "")
    {
        return $"""
        @makima\_daily\_bot это команда! {commandError}
        Запросите список доступных команд ( `@makima_daily_bot list` )
        """;
    }

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
                GetGptChatError(),
                replyToMessageId: message.MessageId,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var receivedCommandName = match.First().Groups[1].Value;
        var currentCommand = _commands.SingleOrDefault(command => command.Name == receivedCommandName);
        if (currentCommand is null)
        {
            await _telegramBotClientWrapper.SendTextMessageAsync(
                chatState.ChatId,
                GetGptChatError($"Команда <**{receivedCommandName.Trim()}**> не распознана!"),
                replyToMessageId: message.MessageId,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: cancellationToken);
            return;
        }

        var rawParameters = match.First().Groups[2].Value;
        await currentCommand.ExecuteAsync(message, chatState, rawParameters, _telegramBotClientWrapper, cancellationToken); 
    }
}
