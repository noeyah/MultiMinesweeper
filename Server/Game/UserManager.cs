using Packet;

namespace Server;

internal class UserManager
{
    private Dictionary<int, User> _dicUser = new Dictionary<int, User>();

    public bool AddUser(int sessionID, string name)
    {
        if (_dicUser.ContainsKey(sessionID))
        {
            return false;
        }

        var user = new User(sessionID);
        user.SetName(name);
        _dicUser.Add(sessionID, user);
        return true;
    }

    public bool RemoveUser(int sessionID)
    {
        if ( _dicUser.ContainsKey(sessionID))
        {
            _dicUser.Remove(sessionID);
            return true;
        }
        return false;
    }

    public IEnumerable<int> GetUsers()
    {
        return _dicUser.Keys;
    }

    public List<Player> GetPlayers()
    {
        return _dicUser.Values.Select(v => v.GetPlayer()).ToList();
    }

    public Player GetPlayer(int sessionID)
    {
        if ( _dicUser.TryGetValue(sessionID, out var user))
        {
            return user.GetPlayer();
        }

        return null;
    }
}
