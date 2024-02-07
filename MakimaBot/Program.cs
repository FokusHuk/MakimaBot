using System.Text.Json;
using Amazon.S3;
using MakimaBot.Model;
using MakimaBot.Model.Config;
using MakimaBot.Model.Events;
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

var telegramBotClient = new TelegramBotClient(applicationConfig.TelegramBotToken);

var chatEvents = new List<IChatEvent>()
{
    new MorningMessageEvent(),
    new ActivityStatisticsEvent()
};
var chatEventsHandler = new ChatEventsHandler(telegramBotClient, chatEvents);
var chatMessagesHandler = new ChatMessagesHandler(telegramBotClient);

var botController = new BotController(telegramBotClient, bucketClient, chatEventsHandler, chatMessagesHandler);

await botController.RunAsync(cts.Token);
