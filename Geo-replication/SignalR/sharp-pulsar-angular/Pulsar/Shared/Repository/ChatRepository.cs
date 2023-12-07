using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Shared.Models;

public sealed class ChatRepository : IChatRepository
{
    private readonly IList<Client> _clients = new List<Client>();
    private readonly ConcurrentDictionary<string, Client> _live = new ConcurrentDictionary<string, Client>();

    private readonly IList<string> _logs = new List<string>();
    private  readonly ConcurrentDictionary<string, List<Post>> _userPosts = new();
    private readonly IList<Post> _posts = new List<Post>();
    private readonly ConcurrentDictionary<string, List<MessageModel>> _messages = new();

    string IChatRepository.Producer { get; set ; }
    string IChatRepository.Consumer { get; set; }

    public void Add(Client client)
    {
        if(_live.ContainsKey(client.Username))
        {
            _clients.Remove(_live[client.Username]);
            _live[client.Username] = client;
            _clients.Add(client);
        }
        else
        {
            _live.TryAdd(client.Username, client);
            _clients.Add(client);
        }  
            
    }
    public void Add(Post post)
    {
        _posts.Add(post);
        //if (!_posts.ContainsKey(post.Username))
            //_posts.TryAdd(post.Username, new List<Post>() {post });
        //else
            //_posts[post.Username].Add(post);
    }
    public void Add(MessageModel message)
    {
        _messages.GetOrAdd(message.Username, (_) => new List<MessageModel>()).Add(message);
    }
    public void Add(string log)
    {
        if (!_logs.Contains(log))
            _logs.Add(log);
    }   

    public Client Get(string connectionId) => _clients.SingleOrDefault(client => client.ConnectionId == connectionId)!;
    public Client GetUser(string username) => _clients.SingleOrDefault(client => client.Username == username)!;

    public IEnumerable<Client> Get() => _clients.OrderBy(client => client.Username);
    public IEnumerable<Post> GetPosts() => _posts;
    public IEnumerable<MessageModel> GetMessages(string username)
    {
        try
        {
            var list = new List<MessageModel>();    
            var m = _messages.Values;
            foreach(var v  in m)
            {
                foreach(var v2 in v)
                {
                    if(v2.Username == username) 
                        list.Add(v2);   
                    else if (v2.Send_Username == username)
                        list.Add(v2);
                }
            }  
            return list!;
        }
        catch
        {
            return null!;
        }
    }

    public IEnumerable<string> StringGet() => _logs;

    public IEnumerable<Client> GetExcept(string connectionId) => Get().Where(client => client.ConnectionId != connectionId);
    public IEnumerable<Client> GetUserExcept(string username) => Get().Where(client => client.Username != username);
    public Client Remove(string connectionId, out IEnumerable<Client> clients)
    {
        var client = Get(connectionId);

        _clients.Remove(client);
        clients = _clients;
        return client;
    }

}
