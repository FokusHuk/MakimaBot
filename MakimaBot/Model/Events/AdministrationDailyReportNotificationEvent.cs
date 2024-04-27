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
        
        var message = GetReportMessage();

        if (!string.IsNullOrWhiteSpace(message))
        {
            await _telegramBotClient.SendTextMessageAsync(
                chat.ChatId,
                message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            _dataContext.UpdateUnknownChatMessages(Array.Empty<UnknownChatMessage>());
            _dataContext.UpdateErrors(Array.Empty<BotError>());
        }

        chat.EventsState.DailyReportNotification.LastTimeStampUtc = DateTime.UtcNow;
    }

    private string GetReportMessage()
    {
        var unknownChatMessages = _dataContext.GetAllUnknownChatMessages();
        var errors = _dataContext.GetAllErrors();

        if (!unknownChatMessages.Any() && !errors.Any())
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

        var errorsReport = string.Join(
            "\n",
            errors.Select(e => $"""
                date time (utc): {e.CreationDateTimeUtc}
                message: {e.Message}
                -----
                """));

        var message = $"""
        *Daily Makima bot report*
        _> Unknown messages_
        {(string.IsNullOrWhiteSpace(unknownMessagesReport) ? "no messages" : unknownMessagesReport)}

        _> Errors_
        {(string.IsNullOrWhiteSpace(errorsReport) ? "no errors" : errorsReport)}
        """;

        return message;
    }
}
