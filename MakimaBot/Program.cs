using System.Text.Json;
using Amazon.S3;
using MakimaBot.Model;
using MakimaBot.Model.Config;
using MakimaBot.Model.Events;
using MakimaBot.Model.Infrastructure;
using Telegram.Bot;

using CancellationTokenSource cts = new();

var configFile = File.ReadAllText("config/config.json");
var applicationConfig = JsonSerializer.Deserialize<ApplicationConfig>(configFile);

if (applicationConfig == null)
{
    throw new InvalidOperationException("Unable to load application config");
}

var s3Config = new AmazonS3Config
{
    ServiceURL = applicationConfig.BucketConfig.ServiceUrl,
    ForcePathStyle = true
};
        
var s3Client = new AmazonS3Client(
    applicationConfig.BucketConfig.AccessKeyId,
    applicationConfig.BucketConfig.SecretAccessKey,
    s3Config);

var bucketClient = new BucketClient(
    s3Client,
    applicationConfig.BucketConfig.BucketName,
    applicationConfig.BucketConfig.StateFileName);

var changelogFile = File.ReadAllText("changelog/changelog.json");
var changelogs = JsonSerializer.Deserialize<Changelog[]>(changelogFile);

var dataContext = new DataContext(bucketClient);
await dataContext.ConfigureAsync();

var telegramBotClient = new TelegramBotClient(applicationConfig.TelegramBotToken);

var chatEvents = new List<IChatEvent>
{
    new MorningMessageEvent(),
    new ActivityStatisticsEvent(),
    new AdministrationDailyReportNotificationEvent(telegramBotClient, dataContext),
    new AppVersionNotificationEvent(changelogs)
};
var chatEventsHandler = new ChatEventsHandler(telegramBotClient, chatEvents, dataContext);
var chatMessagesHandler = new ChatMessagesHandler(telegramBotClient, dataContext);

var infrastructureJobs = new List<InfrastructureJob>
{
    new ErrorsCleanupJob(),
    new UnknownChatMessagesCleanupJob()
};
var infrastructureJobsHandler = new InfrastructureJobsHandler(infrastructureJobs, dataContext);

var botController = new BotController(
    telegramBotClient,
    chatEventsHandler,
    chatMessagesHandler,
    infrastructureJobsHandler);

await botController.RunAsync(cts.Token);
