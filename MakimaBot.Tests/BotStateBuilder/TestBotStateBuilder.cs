using MakimaBot.Model;

namespace MakimaBot.Tests;

public class TestBotStateBuilder
{
    private int? _stateVersion = null;
    private ICollection<ChatState> _chats = [];
    private InfrastructureState? _infrastructure = null;

    public TestBotStateBuilder WithStateVersion(int stateVersion)
    {
        _stateVersion = stateVersion;
        return this;
    }

    public TestBotStateBuilder WithChat(ChatState chatState)
    {
        _chats.Add(chatState);
        return this;
    }

    public TestBotStateBuilder WithInfrastructure(InfrastructureState infrastructureState)
    {
        _infrastructure = infrastructureState;
        return this;
    }

    public BotState Build()
    {
        return new BotState
        {
            StateVersion = _stateVersion ?? TestUniqueValueProvider.GetNextInt(),
            Chats = _chats,
            Infrastructure = _infrastructure ?? new TestInfrastructureStateBuilder().Build()
        };
    }
}
