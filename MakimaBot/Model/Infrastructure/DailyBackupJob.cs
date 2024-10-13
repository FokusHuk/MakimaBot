using System.Text;
using System.Text.Json;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Infrastructure;

public class DailyBackupJob(
    IDataContext dataContext,
    ITelegramBotClientWrapper telegramBotClientWrapper,
    IDateTimeProvider dateTimeProvider) : InfrastructureJob
{
    public override string Name => $"{nameof(DailyBackupJob)}";

    private const string BackupPrefix = "makima-backup";
    private const string BackupFileType = "txt";

    public override async Task ExecuteAsync()
    {
        var backupJobState = dataContext.State.Infrastructure.DailyBackupJobState;
        var currentDateUtc = dateTimeProvider.UtcNow().Date;

        if (backupJobState is null || currentDateUtc == backupJobState.LastTimeStampUtc.Date)
            return;

        var jsonState = JsonSerializer.Serialize(dataContext.State);
        var jsonStateBytes = Encoding.UTF8.GetBytes(jsonState);
        var stream = new MemoryStream(jsonStateBytes);

        var backupFile = InputFile.FromStream(stream, GetBackupFileName(currentDateUtc));

        await telegramBotClientWrapper.SendDocumentAsync(backupJobState.TargetChatId, backupFile);

        backupJobState.LastTimeStampUtc = currentDateUtc;
        await dataContext.SaveChangesAsync();
    }

    private string GetBackupFileName(DateTime currentDateUtc) => $"{BackupPrefix}-{currentDateUtc:dd-MM-yyyy}.{BackupFileType}";
}
