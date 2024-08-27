using System.Text.Json;
using Amazon.S3;
using MakimaBot.Model;
using MakimaBot.Model.Config;
using MakimaBot.Model.Events;
using MakimaBot.Model.Infrastructure;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var configFile = File.ReadAllText("config/config.json");
        var applicationConfig = JsonSerializer.Deserialize<ApplicationConfig>(configFile)
            ?? throw new InvalidOperationException("Unable to load application config");

        var changelogFile = File.ReadAllText("changelog/changelog.json");
        var changelogs = JsonSerializer.Deserialize<Changelog[]>(changelogFile)
            ?? throw new InvalidOperationException("Unable to load changelog file");


        services.AddSingleton<BucketClient>(provider =>
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

        services.AddSingleton<DataContext>();

        services.AddSingleton<TelegramBotClient>(provider => 
            new TelegramBotClient(applicationConfig.TelegramBotToken));


        services.AddSingleton<IChatEvent, MorningMessageEvent>();
        services.AddSingleton<IChatEvent, EveningMessageEvent>();
        services.AddSingleton<IChatEvent, DailyActivityStatisticsEvent>();
        services.AddSingleton<IChatEvent, MonthlyActivityStatisticsEvent>();
        services.AddSingleton<IChatEvent, AdministrationDailyReportNotificationEvent>();
        services.AddSingleton<IChatEvent, AppVersionNotificationEvent>(provider =>
            new AppVersionNotificationEvent(changelogs));

        services.AddSingleton<ChatEventsHandler>();
        services.AddSingleton<ChatMessagesHandler>();

        // temporarily disable - cleaned up in daily report event now
        //services.AddSingleton<InfrastructureJob, ErrorsCleanupJob>();
        //services.AddSingleton<InfrastructureJob, UnknownChatMessagesCleanupJob>();

        services.AddSingleton<InfrastructureJobsHandler>();

        services.AddSingleton<IBotService, BotService>();

        services.AddHttpClient();

        services.AddSingleton<ChatCommandHandler>();
        services.AddSingleton<ChatCommand, GptChatCommand>();
        services.AddSingleton<IGptClient, GptClient>(provider =>
        {
            var httpClientFactory = provider.GetService<IHttpClientFactory>();
            return new GptClient(httpClientFactory, applicationConfig.GptConfig);
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
