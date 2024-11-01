using MakimaBot.Model;

namespace MakimaBot.Tests;

public class TestUserStatesBuilder
{
    private ICollection<UserState> _userStates = [];

    public TestUserStatesBuilder WithUser(long userId, string userName, ICollection<string> allowedCommands)
    {
        _userStates.Add(
            new UserState
            {
                UserId = userId,
                UserName = userName,
                AllowedCommands = allowedCommands
            });
        return this;
    }

    public ICollection<UserState> Build()
    {
        return _userStates;
    }
}