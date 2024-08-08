using System.Text.Json;
using Amazon.S3;
using MakimaBot.Model;
using MakimaBot.Model.Config;
using MakimaBot.Model.Events;
using MakimaBot.Model.Infrastructure;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var configFile = File.ReadAllText("config/config.json");
var applicationConfig = JsonSerializer.Deserialize<ApplicationConfig>(configFile)
    ?? throw new InvalidOperationException("Unable to load application config");

var changelogFile = File.ReadAllText("changelog/changelog.json");
var changelogs = JsonSerializer.Deserialize<Changelog[]>(changelogFile)
    ?? throw new InvalidOperationException("Unable to load changelog file");


var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddSingleton<BucketClient>(provider =>
{
    var s3Config = new AmazonS3Config
    {
        ServiceURL = applicationConfig.BucketConfig.ServiceUrl,
        ForcePathStyle = true
    };
        
    var s3Client = new AmazonS3Client(
        applicationConfig.BucketConfig.AccessKeyId,
        applicationConfig.BucketConfig.SecretAccessKey,
        s3Config);

    return new BucketClient(
        s3Client,
        applicationConfig.BucketConfig.BucketName,
        applicationConfig.BucketConfig.StateFileName);
});

builder.Services.AddSingleton<DataContext>();

builder.Services.AddSingleton<TelegramBotClient>(provider => 
    new TelegramBotClient(applicationConfig.TelegramBotToken));


builder.Services.AddSingleton<IChatEvent, MorningMessageEvent>();
builder.Services.AddSingleton<IChatEvent, ActivityStatisticsEvent>();
builder.Services.AddSingleton<IChatEvent, AdministrationDailyReportNotificationEvent>();
builder.Services.AddSingleton<IChatEvent, AppVersionNotificationEvent>(provider =>
    new AppVersionNotificationEvent(changelogs));

builder.Services.AddSingleton<ChatEventsHandler>();
builder.Services.AddSingleton<ChatMessagesHandler>();


builder.Services.AddSingleton<InfrastructureJob, ErrorsCleanupJob>();
builder.Services.AddSingleton<InfrastructureJob, UnknownChatMessagesCleanupJob>();

builder.Services.AddSingleton<InfrastructureJobsHandler>();


builder.Services.AddHostedService<BotController>();
using var host = builder.Build();
await host.RunAsync();
