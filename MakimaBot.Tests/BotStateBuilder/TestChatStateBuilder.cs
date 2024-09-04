using MakimaBot.Model;

namespace MakimaBot.Tests;

public class TestChatStateBuilder
{
    private long? _chatId = null;
    private string? _name = null;
    private EventsState? _eventsState = null;

    public TestChatStateBuilder WithId(long chatId)
    {
        _chatId = chatId;
        return this;
    }

    public TestChatStateBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TestChatStateBuilder WithEventState(EventsState eventsState)
    {
        _eventsState = eventsState;
        return this;
    }

    public ChatState Build()
    {
        return new ChatState
        {
            ChatId = _chatId ?? TestUniqueValueProvider.GetNextLong(),
            Name = _name ?? TestUniqueValueProvider.GetNextString("chat_name"),
            EventsState = _eventsState ?? new TestEventsStateBuilder().Build()
        };
    }
}
