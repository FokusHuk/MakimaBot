namespace MakimaBot.Model.Infrastructure;

public class BotNotificationsJob(
    IDataContext dataContext,
    ITelegramBotClientWrapper telegramBotClientWrapper,
    IDateTimeProvider dateTimeProvider) : InfrastructureJob
{
    public override string Name => $"{nameof(BotNotificationsJob)}";

    protected virtual int MaxTelegramMessageLength { get; } = 4096;

    protected virtual string ErrorsReportTitle { get; } = "Errors";
    protected virtual Func<BotError, string> ErrorReportProvider { get; } = error => $"""
        -----
        last seen (utc): {error.LastSeenDateTimeUtc}
        message: {error.Message}
        count: {error.Count}
        """;

    protected virtual string UnknownMessagesReportTitle { get; } = "Errors";
    protected virtual Func<UnknownChatMessage, string> UnknownMessageReportProvider { get; } = ucm => $"""
        -----
        chat id: {ucm.ChatId}
        name: {ucm.Name}
        message: {ucm.Message}
        send datetime (utc): {ucm.SentDateTimeUtc}
        """;

    public override async Task ExecuteAsync()
    {   
        var notificationsSettings = dataContext.State.Infrastructure.NotificationsChatSettings;

        if (notificationsSettings is null)
            return;

        var errors = dataContext.GetAllErrors().ToList();
        var (errorsReport, restErrors) = GetReport(
            ErrorsReportTitle,
            errors,
            ErrorReportProvider);

        var unknownChatMessages = dataContext.GetAllUnknownChatMessages().ToList();
        var (unknownMessagesReport, restUnknownMessages) = GetReport(
            UnknownMessagesReportTitle,
            unknownChatMessages,
            UnknownMessageReportProvider);

        if (!string.IsNullOrWhiteSpace(errorsReport))
        {
            await telegramBotClientWrapper.SendTextMessageAsync(
                notificationsSettings.ChatId,
                unknownMessagesReport);

            dataContext.UpdateErrors(restErrors);
            await dataContext.SaveChangesAsync();
        }

        if (!string.IsNullOrWhiteSpace(unknownMessagesReport))
        {
            await telegramBotClientWrapper.SendTextMessageAsync(
                notificationsSettings.ChatId,
                unknownMessagesReport);

            dataContext.UpdateUnknownChatMessages(restUnknownMessages);
            await dataContext.SaveChangesAsync();
        }


        var currentDateUtc = dateTimeProvider.UtcNow().Date;
        if(currentDateUtc != notificationsSettings.LastHealthCheckTimeStampUtc.Date)
        {
            await telegramBotClientWrapper.SendTextMessageAsync(
                notificationsSettings.ChatId,
                text: $"_Friendly health check to remind you I'm still alive_ ❤️",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            notificationsSettings.LastHealthCheckTimeStampUtc = currentDateUtc;
            await dataContext.SaveChangesAsync();
        }
    }

    private (string Message, ICollection<T> RestErrors) GetReport<T>(
        string title,
        List<T> entities,
        Func<T, string> entityReportProvider)
    {
        var report = string.Empty;

        if (entities.Count == 0)
            return (report, entities);
        
        var processedEntities = new List<T>();
        foreach (var entity in entities)
        {
            var entityReport = entityReportProvider(entity);
            
            if (GetFullReport($"{report}\n{entityReport}").Length > MaxTelegramMessageLength)
            {
                break;
            }

            report += $"\n{entityReport}";
            processedEntities.Add(entity);
        }

        return (GetFullReport(report), entities.Except(processedEntities).ToList());

        string GetFullReport(string entitiesReport) =>
            $"""
            > {title}
            {entitiesReport}
            """;
    }
}
