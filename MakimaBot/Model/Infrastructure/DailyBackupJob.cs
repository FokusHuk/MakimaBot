using System.Text;
using System.Text.Json;
using Telegram.Bot.Types;

namespace MakimaBot.Model.Infrastructure;

public class DailyBackupJob(
    IDataContext dataContext,
    ITelegramBotClientWrapper telegramBotClientWrapper) : InfrastructureJob
{
    public override string Name => $"{nameof(DailyBackupJob)}";

    private const string BackupPrefix = "makima-backup";
    private const string BackupFileType = "txt";

    public override async Task ExecuteAsync()
    {
        var state = dataContext.State;
        var currentDateUtc = DateTime.UtcNow.Date;

        var lastStartTimeStampUtc = dataContext.State.Infrastructure.DailyBackupJobState.LastTimeStampUtc;
        if (currentDateUtc == lastStartTimeStampUtc.Date)
            return;

        var adminChat = state.Chats.First(c => c.Name == "akima_yooukie");

        var jsonState = JsonSerializer.Serialize(state);
        var jsonStateBytes = Encoding.UTF8.GetBytes(jsonState);
        var stream = new MemoryStream(jsonStateBytes);

        var backupFile = InputFile.FromStream(stream, GetBackupFileName(currentDateUtc));

        await telegramBotClientWrapper.SendDocumentAsync(adminChat.ChatId, backupFile);

        state.Infrastructure.DailyBackupJobState.LastTimeStampUtc = currentDateUtc;
        await dataContext.SaveChangesAsync();
    }

    private string GetBackupFileName(DateTime currentDateUtc) => $"{BackupPrefix}-{currentDateUtc.ToShortDateString()}.{BackupFileType}";
}
