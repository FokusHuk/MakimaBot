namespace MakimaBot.Model.Events;

public class AdministrationDailyReportNotificationEvent : IChatEvent
{
    private readonly IDataContext _dataContext;
    private readonly TimeSpan notificationTimeStartUtc = new TimeSpan(hours: 18, minutes: 0, seconds: 0);
    private readonly TimeSpan notificationTimeEndUtc = new TimeSpan(hours: 18, minutes: 30, seconds: 0);

    public AdministrationDailyReportNotificationEvent(IDataContext dataContext)
    {
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

    public async Task HandleEventAsync(ITelegramBotClientWrapper telegramBotClientWrapper, ChatState chat)
    {
        if (chat?.Name != "akima_yooukie")
            return;
        
        var errorsReport = GetErrorsReport();
        var unknownMessagesReport = GetUnknownMessagesReport();
        

        if (!string.IsNullOrWhiteSpace(errorsReport))
        {
            await telegramBotClientWrapper.SendTextMessageAsync(
                chat.ChatId,
                TrimTelegramMessage(errorsReport));

            _dataContext.FlushErrors();
        }

        if (!string.IsNullOrWhiteSpace(unknownMessagesReport))
        {
            await telegramBotClientWrapper.SendTextMessageAsync(
                chat.ChatId,
                TrimTelegramMessage(unknownMessagesReport));

            _dataContext.FlushUnknownChatMessages();
        }

        if(string.IsNullOrWhiteSpace(unknownMessagesReport) && string.IsNullOrWhiteSpace(errorsReport))
        {
            var message = $"""
            *Daily Makima bot report* 
            Поздравляю, таска не сдохла!!!! Просто сегодня не было ни новых рандомных типОв, ни ошибок.  
            """;
            await telegramBotClientWrapper.SendTextMessageAsync(
                chat.ChatId,
                message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
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
