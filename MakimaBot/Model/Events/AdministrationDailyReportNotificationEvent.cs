using System.Globalization;
using System.Text.Json;
using Telegram.Bot;

namespace MakimaBot.Model.Events;

public class AdministrationDailyReportNotificationEvent : IChatEvent
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly DataContext _dataContext;

    private readonly TimeSpan notificationTimeStartUtc =
        DateTime.Parse("2023-01-01 18:00:00", CultureInfo.InvariantCulture).TimeOfDay;

    private readonly TimeSpan notificationTimeEndUtc =
        DateTime.Parse("2023-01-01 18:30:00", CultureInfo.InvariantCulture).TimeOfDay;

    public AdministrationDailyReportNotificationEvent(
        TelegramBotClient telegramBotClient,
        DataContext dataContext)
    {
        _telegramBotClient = telegramBotClient;
        _dataContext = dataContext;
    }

    public bool ShouldLaunch(ChatState chat)
    {
        var currentDateTimeUtc = DateTime.UtcNow;

        return chat.EventsState.DailyReportNotification.IsEnabled
               && currentDateTimeUtc.Date != chat.EventsState.DailyReportNotification.LastTimeStampUtc.Date
               && currentDateTimeUtc.TimeOfDay > notificationTimeStartUtc
               && currentDateTimeUtc.TimeOfDay < notificationTimeEndUtc;
    }

    public async Task HandleEventAsync(TelegramBotClient telegramBotClient, ChatState chat)
    {
        if (chat?.Name != "akima_yooukie")
            return;
        
        var errorsReport = GetErrorsReport();
        var unknownMessagesReport = GetUnknownMessagesReport();
        

        if (!string.IsNullOrWhiteSpace(errorsReport))
        {
            await _telegramBotClient.SendTextMessageAsync(
                chat.ChatId,
                TrimTelegramMessage(errorsReport),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            _dataContext.UpdateErrors(Array.Empty<BotError>());
        }

        if (!string.IsNullOrWhiteSpace(unknownMessagesReport))
        {
            await _telegramBotClient.SendTextMessageAsync(
                chat.ChatId,
                TrimTelegramMessage(unknownMessagesReport),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            _dataContext.UpdateUnknownChatMessages(Array.Empty<UnknownChatMessage>());
        }

        chat.EventsState.DailyReportNotification.LastTimeStampUtc = DateTime.UtcNow;
    }

    private string GetErrorsReport()
    {
        var errors = _dataContext.GetAllErrors();

        if (!errors.Any())
            return string.Empty;

        var errorsReport = string.Join(
            "\n",
            errors.Select(e => $"""
                last seen (utc): {e.LastSeenDateTimeUtc}
                message: {e.Message}
                count: {e.Count}
                -----
                """));

        var message = $"""
        *Daily Makima bot report*
        _> Errors_
        {(string.IsNullOrWhiteSpace(errorsReport) ? "no errors" : errorsReport)}
        """;

        return message;
    }

    private string GetUnknownMessagesReport()
    {
        var unknownChatMessages = _dataContext.GetAllUnknownChatMessages();

        if (!unknownChatMessages.Any())
            return string.Empty;

        var unknownMessagesReport = string.Join(
            "\n",
            unknownChatMessages.Select(m => $"""
                chat id: {m.ChatId}
                name: {m.Name}
                message: {m.Message}
                send datetime (utc): {m.SentDateTimeUtc}
                -----
                """));

        var message = $"""
        *Daily Makima bot report*
        _> Unknown messages_
        {(string.IsNullOrWhiteSpace(unknownMessagesReport) ? "no messages" : unknownMessagesReport)}
        """;

        return message;
    }

    private string TrimTelegramMessage(string message)
    {
        const int MaxTelegramMessageLength = 4096;

        if (message.Length > MaxTelegramMessageLength)
            message = message[..MaxTelegramMessageLength];

        return message;
    }
}
