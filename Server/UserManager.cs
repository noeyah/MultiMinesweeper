using System.Collections.Concurrent;

namespace Server;

internal class UserManager
{
    private readonly ConcurrentDictionary<int, User> _dicUser = new();

    public bool AddUser(int sessionID, string name)
    {
        var user = new User(sessionID, name);
        return _dicUser.TryAdd(sessionID, user);
    }

    public bool RemoveUser(int sessionID)
    {
        return _dicUser.TryRemove(sessionID, out _);
    }

    public User? GetUser(int sessionID)
    {
        if (_dicUser.TryGetValue(sessionID, out var user))
        {
            return user;
        }

        return null;
    }
}
