using MakimaBot.Model;
using MakimaBot.Model.Infrastructure;
using Moq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MakimaBot.Tests;

[TestClass]
public class BotNotificationsJobTests
{
    [TestMethod]
    public async Task ExecuteAsync_HasErrorsAndUnknownChatMessages_HealthCheckTriggered_SentFullReport()
    {
        var currentDateUtc = DateTime.Parse("2024-10-18");

        var state = CreateState(currentDateUtc.AddDays(-1));

        var botErrors = GetBotErrors(1, 10);
        var unknownChatMessages = GetUnknownChatMessages(1, 10);

        var dataContext = GetDataContextMock(state, botErrors, unknownChatMessages);

        var telegramBotClientMock = GetTelegramBotClientWrapperMock();

        var job = new TestBotNotificationsJob(dataContext.Object, telegramBotClientMock.Object, GetDateTimeProvider(currentDateUtc));


        await job.ExecuteAsync();


        Assert.IsTrue(botErrors.Count == 0);
        Assert.IsTrue(unknownChatMessages.Count == 0);
        dataContext.Verify(dc => dc.SaveChangesAsync(), Times.Exactly(3));
        telegramBotClientMock.Verify(tbcw => tbcw.SendTextMessageAsync(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.IsAny<ParseMode?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(3));
        Assert.AreEqual(currentDateUtc, state.Infrastructure.NotificationsChatSettings.LastHealthCheckTimeStampUtc);
    }

    [TestMethod]
    public async Task ExecuteAsync_HasOnlyErrors_NoHealthCheck_SentErrorsReport()
    {
        var currentDateUtc = DateTime.Parse("2024-10-18");

        var state = CreateState(currentDateUtc);

        var botErrors = GetBotErrors(1, 10);

        var dataContext = GetDataContextMock(state, botErrors, []);

        var telegramBotClientMock = GetTelegramBotClientWrapperMock();

        var job = new TestBotNotificationsJob(dataContext.Object, telegramBotClientMock.Object, GetDateTimeProvider(currentDateUtc));


        await job.ExecuteAsync();


        Assert.IsTrue(botErrors.Count == 0);
        dataContext.Verify(dc => dc.SaveChangesAsync(), Times.Exactly(1));
        telegramBotClientMock.Verify(tbcw => tbcw.SendTextMessageAsync(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.IsAny<ParseMode?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(1));
    }

    [TestMethod]
    public async Task ExecuteAsync_HasOnlyUnknownChatMessages_NoHealthCheck_SentUnknownChatMessagesReport()
    {
        var currentDateUtc = DateTime.Parse("2024-10-18");

        var state = CreateState(currentDateUtc);

         var unknownChatMessages = GetUnknownChatMessages(1, 10);

        var dataContext = GetDataContextMock(state, [], unknownChatMessages);

        var telegramBotClientMock = GetTelegramBotClientWrapperMock();

        var job = new TestBotNotificationsJob(dataContext.Object, telegramBotClientMock.Object, GetDateTimeProvider(currentDateUtc));


        await job.ExecuteAsync();


        Assert.IsTrue(unknownChatMessages.Count == 0);
        dataContext.Verify(dc => dc.SaveChangesAsync(), Times.Exactly(1));
        telegramBotClientMock.Verify(tbcw => tbcw.SendTextMessageAsync(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.IsAny<ParseMode?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(1));
    }

    [TestMethod]
    public async Task ExecuteAsync_NoErrorsAndUnknownChatMessages_HealthCheckTriggered_SentHealthCheckReport()
    {
        var currentDateUtc = DateTime.Parse("2024-10-18");

        var state = CreateState(currentDateUtc.AddDays(-1));

        var dataContext = GetDataContextMock(state, [], []);

        var telegramBotClientMock = GetTelegramBotClientWrapperMock();

        var job = new TestBotNotificationsJob(dataContext.Object, telegramBotClientMock.Object, GetDateTimeProvider(currentDateUtc));


        await job.ExecuteAsync();


        dataContext.Verify(dc => dc.SaveChangesAsync(), Times.Exactly(1));
        telegramBotClientMock.Verify(tbcw => tbcw.SendTextMessageAsync(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.IsAny<ParseMode?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(1));
        Assert.AreEqual(currentDateUtc, state.Infrastructure.NotificationsChatSettings.LastHealthCheckTimeStampUtc);


        await job.ExecuteAsync();


        dataContext.Verify(dc => dc.SaveChangesAsync(), Times.Exactly(1));
        telegramBotClientMock.Verify(tbcw => tbcw.SendTextMessageAsync(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.IsAny<ParseMode?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(1));
        Assert.AreEqual(currentDateUtc, state.Infrastructure.NotificationsChatSettings.LastHealthCheckTimeStampUtc);
    }

    [TestMethod]
    public async Task ExecuteAsync_HasOnlyErrors_ErrorsReportExceedsMessageSizeLimit_SentErrorsReportsByTwoExecutions()
    {
        var currentDateUtc = DateTime.Parse("2024-10-18");

        var state = CreateState(currentDateUtc);

        var botErrors = GetBotErrors(2, 80);

        var dataContext = GetDataContextMock(state, botErrors, []);

        var telegramBotClientMock = GetTelegramBotClientWrapperMock();

        var job = new TestBotNotificationsJob(dataContext.Object, telegramBotClientMock.Object, GetDateTimeProvider(currentDateUtc));


        await job.ExecuteAsync();


        Assert.IsTrue(botErrors.Count == 1);
        dataContext.Verify(dc => dc.SaveChangesAsync(), Times.Exactly(1));
        telegramBotClientMock.Verify(tbcw => tbcw.SendTextMessageAsync(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.IsAny<ParseMode?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(1));


        await job.ExecuteAsync();


        Assert.IsTrue(botErrors.Count == 0);
        dataContext.Verify(dc => dc.SaveChangesAsync(), Times.Exactly(2));
        telegramBotClientMock.Verify(tbcw => tbcw.SendTextMessageAsync(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.IsAny<ParseMode?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(2));
    }

    private BotState CreateState(DateTime currentDateTimeUtc) =>
        new TestBotStateBuilder()
            .WithChat(new TestChatStateBuilder()
                .Build())
            .WithInfrastructure(new TestInfrastructureStateBuilder()
                .WithNotificationsChatSettings(new ()
                {
                    ChatId = 1,
                    LastHealthCheckTimeStampUtc = currentDateTimeUtc
                })
                .Build())
            .Build();

    private List<BotError> GetBotErrors(int errorsCount, int messagesLength)
    {
        return Enumerable
            .Range(0, errorsCount)
            .Select(_ => new BotError
            {
                Message = new string('a', messagesLength),
                LastSeenDateTimeUtc = DateTime.UtcNow,
                Count = 1
            })
            .ToList();
    }

    private List<UnknownChatMessage> GetUnknownChatMessages(int messagesCount, int messagesLength)
    {
        return Enumerable
            .Range(0, messagesCount)
            .Select(_ => new UnknownChatMessage
            {
                ChatId = 1,
                Message = new string('a', messagesLength),
                Name = "TestUser",
                SentDateTimeUtc = DateTime.UtcNow
            })
            .ToList();
    }

    private Mock<IDataContext> GetDataContextMock(
        BotState state,
        List<BotError> botErrors,
        List<UnknownChatMessage> unknownChatMessages)
    {
        var dataContext = new Mock<IDataContext>();

        dataContext
            .Setup(dc => dc.State)
            .Returns(state);

        dataContext
            .Setup(dc => dc.GetAllErrors())
            .Returns(botErrors);
        dataContext
            .Setup(dc => dc.UpdateErrors(It.IsAny<ICollection<BotError>>()))
            .Callback<ICollection<BotError>>(errors => {
                botErrors.Clear();
                botErrors.AddRange(errors);
            });

        dataContext
            .Setup(dc => dc.GetAllUnknownChatMessages())
            .Returns(unknownChatMessages);
        dataContext
            .Setup(dc => dc.UpdateUnknownChatMessages(It.IsAny<ICollection<UnknownChatMessage>>()))
            .Callback<ICollection<UnknownChatMessage>>(messages => {
                unknownChatMessages.Clear();
                unknownChatMessages.AddRange(messages);
            });

        dataContext
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(true)
            .Verifiable();

        return dataContext;
    }

    private Mock<ITelegramBotClientWrapper> GetTelegramBotClientWrapperMock()
    {
        var telegramBotClientMock = new Mock<ITelegramBotClientWrapper>();
        telegramBotClientMock
            .Setup(tbcw => tbcw.SendTextMessageAsync(
                It.IsAny<ChatId>(),
                It.IsAny<string>(),
                It.IsAny<ParseMode?>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            ))
            .Verifiable();
        return telegramBotClientMock;
    }

    private IDateTimeProvider GetDateTimeProvider(DateTime currentDateUtc)
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider
            .Setup(dt => dt.UtcNow())
            .Returns(currentDateUtc);
        return dateTimeProvider.Object;
    }
}

file class TestBotNotificationsJob : BotNotificationsJob
{
    public TestBotNotificationsJob(
        IDataContext dataContext,
        ITelegramBotClientWrapper telegramBotClientWrapper,
        IDateTimeProvider dateTimeProvider) : base(dataContext, telegramBotClientWrapper, dateTimeProvider)
    {
    }

    protected override int MaxTelegramMessageLength => 100;

    protected override string ErrorsReportTitle => "ErrorsTest";
    protected override Func<BotError, string> ErrorReportProvider => error => error.Message;

    protected override string UnknownMessagesReportTitle => "UnknownMessagesTest";
    protected override Func<UnknownChatMessage, string> UnknownMessageReportProvider => ucm => ucm.Message;
}
