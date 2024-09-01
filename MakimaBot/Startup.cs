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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MakimaBot.Model.Processors;

public class Startup(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddOptions<ApplicationOptions>()
            .Bind(_configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<ChangelogOptions>()
            .Bind(_configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<TelegramOptions>()
            .Bind(_configuration.GetSection(TelegramOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<BucketOptions>()
            .Bind(_configuration.GetSection(BucketOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<GptOptions>()
            .Bind(_configuration.GetSection(GptOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();


        services.AddSingleton<IBucketClient, BucketClient>(provider =>
        {
            var bucketOptions = provider.GetRequiredService<IOptions<BucketOptions>>().Value;

            var s3Config = new AmazonS3Config
            {
                ServiceURL = bucketOptions.ServiceUrl,
                ForcePathStyle = true
            };
                
            var s3Client = new AmazonS3Client(
                bucketOptions.AccessKeyId,
                bucketOptions.SecretAccessKey,
                s3Config);

            return new BucketClient(
                s3Client,
                bucketOptions.BucketName,
                bucketOptions.StateFileName);
        });

        services.AddSingleton<DataContext>();

        services.AddSingleton<TelegramBotClient>(provider =>
        {
            var telegramOptions = provider.GetRequiredService<IOptions<TelegramOptions>>().Value;
            return new TelegramBotClient(telegramOptions.Token);
        });


        services.AddSingleton<IChatEvent, MorningMessageEvent>();
        services.AddSingleton<IChatEvent, EveningMessageEvent>();
        services.AddSingleton<IChatEvent, ActivityStatisticsEvent>();
        services.AddSingleton<IChatEvent, AdministrationDailyReportNotificationEvent>();
        services.AddSingleton<IChatEvent, AppVersionNotificationEvent>();

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
        services.AddSingleton<IGptClient, GptClient>();

        services.AddSingleton<BotStateUpdater>();
        
        services.AddSingleton<ITextDiffPrinter, ConsoleTextDiffPrinter>();
        
        services.AddSingleton<Migration, TestAddMigration>();


        services.AddTransient<DailyActivityProcessor>();
        services.AddTransient<GptMessageProcessor>();
        services.AddTransient<HealthCheackProcessor>();
        services.AddTransient<RandomPhraseProcessor>();
        services.AddTransient<TrustedChatProcessor>();
        services.AddTransient<UntrustedChatProcessor>();

        services.AddSingleton<ProcessorComponent>();
        services.AddSingleton<ProcessorsChainFactory, DefaultProcessorsChainFactory>();


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
