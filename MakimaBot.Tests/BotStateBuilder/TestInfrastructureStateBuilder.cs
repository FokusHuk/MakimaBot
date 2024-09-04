using MakimaBot.Model;

namespace MakimaBot.Tests;

public class TestInfrastructureStateBuilder
{
    private ICollection<BotError> _errors = [];
    private ICollection<UnknownChatMessage> _unknownChatsMessages = [];

    public TestInfrastructureStateBuilder WithError(BotError botError)
    {
        _errors.Add(botError);
        return this;
    }

    public TestInfrastructureStateBuilder WithUnknownChatMessage(UnknownChatMessage unknownChatMessage)
    {
        _unknownChatsMessages.Add(unknownChatMessage);
        return this;
    }

    public InfrastructureState Build()
    {
        return new InfrastructureState
        {
            Errors = _errors,
            UnknownChatsMessages = _unknownChatsMessages
        };
    }
}
