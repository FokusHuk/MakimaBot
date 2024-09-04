using MakimaBot.Model;

namespace MakimaBot.Tests;

[TestClass]
public class BotStateBuilderExamples
{
    [TestMethod]
    public async Task BotStateBuilderExample_Simple()
    {
        var botState = new TestBotStateBuilder().Build();
    }

    [TestMethod]
    public async Task BotStateBuilderExample_Full()
    {
        var botState = new TestBotStateBuilder()
            .WithStateVersion(5)
            .WithInfrastructure(new TestInfrastructureStateBuilder()
                .WithError(new BotError()
                {
                    Message = "test error",
                    LastSeenDateTimeUtc = DateTime.UtcNow,
                    Count = 10
                })
                .WithUnknownChatMessage(new UnknownChatMessage()
                {
                    ChatId = 7,
                    Name = "user",
                    Message = "/start",
                    SentDateTimeUtc = DateTime.UtcNow
                })
                .Build())
            .WithChat(new TestChatStateBuilder()
                .WithId(7)
                .WithName("test makima chat")
                .WithEventState(new TestEventsStateBuilder()
                    .WithMorningMessageEventState(new()
                    {
                        IsEnabled = true,
                        LastTimeStampUtc = DateTime.UtcNow,
                        NextStartTimeStampUtc = DateTime.UtcNow
                    })
                    .WithActivityStatisticsEventState(new()
                    {
                        IsEnabled = true,
                        LastTimeStampUtc = DateTime.UtcNow,
                        Statistics = new()
                        {
                            { 7515, 10 }
                        }
                    })
                    .WithEveningMessageEventState(new()
                    {
                        IsEnabled = true,
                        LastTimeStampUtc = DateTime.UtcNow,
                        NextStartTimeStampUtc = DateTime.UtcNow
                    })
                    .Build())
                .Build())
            .Build();
    }

    [TestMethod]
    public async Task BotStateBuilderExample_Partial()
    {
        var botState = new TestBotStateBuilder()
            .WithStateVersion(5)
            .WithInfrastructure(new TestInfrastructureStateBuilder()
                .WithUnknownChatMessage(new UnknownChatMessage()
                {
                    ChatId = 7,
                    Name = "user",
                    Message = "/start",
                    SentDateTimeUtc = DateTime.UtcNow
                })
                .WithUnknownChatMessage(new UnknownChatMessage()
                {
                    ChatId = 8,
                    Name = "user2",
                    Message = "hello",
                    SentDateTimeUtc = DateTime.UtcNow.AddDays(-1)
                })
                .Build())
            .Build();
    }
}
