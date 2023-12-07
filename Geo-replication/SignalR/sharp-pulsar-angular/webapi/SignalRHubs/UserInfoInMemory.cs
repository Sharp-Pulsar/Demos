using Shared.Models;
using System.Collections.Concurrent;

namespace webapi.SignalRHubs;

public class UserInfoInMemory
{
    private readonly ConcurrentDictionary<string, Client> _onlineUser = new();

    public bool AddUpdate(string? name, string? password, string connectionId)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
        {
            var userAlreadyExists = _onlineUser.ContainsKey(name);

            var userInfo = new Client()
            {
                ConnectionId = connectionId,
                Username = name,
                Password = password
            }; 

            _onlineUser.AddOrUpdate(name, userInfo, (key, value) => userInfo);

            return userAlreadyExists;
        }

        throw new ArgumentNullException(nameof(name));
    }

    public void Remove(string? name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            _onlineUser.TryRemove(name, out _);
        }
    }

    public IEnumerable<Client> GetAllUsersExceptThis(string? username)
    {
        if (string.IsNullOrEmpty(username))
            return new List<Client>();

        return _onlineUser.Values.Where(item => item.Username != username);
    }

    public Client GetUserInfo(string? username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            _onlineUser.TryGetValue(username, out Client? userInfo);
            if (userInfo != null)
                return userInfo;
        }

        throw new ArgumentNullException(nameof(username));
    }
}
