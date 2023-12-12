using Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace webapi;
public class PulsarHub : Hub
{
    private readonly IChatRepository _chatRepository;

    public PulsarHub(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }
    public override async Task OnConnectedAsync()
    {
        Context.GetHttpContext().Request.Query.TryGetValue("username", out var name);
        Context.GetHttpContext().Request.Query.TryGetValue("password", out var pass);
        if (name == "producer" && pass == "producer")
        {

            Console.WriteLine($"{name} {pass}");
            _chatRepository.Producer = Context.ConnectionId;
            Console.WriteLine($"PRODUCER CONNECTIONID {_chatRepository.Producer}");
        }
        else if (name == "consumer" && pass == "consumer")
        {
            Console.WriteLine($"{name} {pass}");
            _chatRepository.Consumer = Context.ConnectionId;
            Console.WriteLine($"CONSUMER CONNECTIONID {_chatRepository.Consumer}");
        }
        else
        {
            var client = new Client()
            {
                ConnectionId = Context.ConnectionId,
                Username = name,
                Password = pass
            };
            await Clients.Client(_chatRepository.Producer).SendAsync("Usernamed", client);
            //var client = JsonSerializer.Deserialize<Client>(c);
            _chatRepository.Add(client);

            await Clients.Client(Context.ConnectionId).SendAsync("Connected", new Connected(client));

        }

        //pulsar read
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var client = _chatRepository.Remove(Context.ConnectionId, out var clients);

        await Clients.Client(Context.ConnectionId).SendAsync("Disconnected", new Disconnected(client));
        await Clients.All.SendAsync("Loaded", new Loaded(clients));
        await base.OnDisconnectedAsync(exception);
    }
    public async Task Post(Post post)
    {
        //await _processor.Post(post);
        await Clients.Client(_chatRepository.Producer).SendAsync("Posted", post);
    }
    public async Task Logged(string s)
    {
        _chatRepository.Add(s);
        var logs = _chatRepository.StringGet();
        await Clients.All.SendAsync("Logged", new Logged(logs));
    }
    public async Task ConsumerUsername(Client client)
    {

        //var clients = _chatRepository.GetExcept(Context.ConnectionId);
        _chatRepository.Add(client);
        var clients = _chatRepository.Get();
        await Clients.All.SendAsync("Loaded", new Loaded(clients));

        #region username
        var post = _chatRepository.GetPosts();
        if (post != null)
        {
            await Clients.All.SendAsync("Posted", new Posted(post));
        }
        var message = _chatRepository.GetMessages(client.Username);
        if (message != null)
        {
            await Clients.Client(client.ConnectionId).SendAsync("Messages", new Messaged(message));
        }
        #endregion

    }
    public async Task ConsumerPost(Post p)
    {
        //var p = JsonSerializer.Deserialize<Post>(pt);
        _chatRepository.Add(p);
        var post = _chatRepository.GetPosts();
        await Clients.All.SendAsync("Posted", post);
    }
    public async Task ConsumerMessage(MessageModel m)
    {
        //var msg = JsonSerializer.Deserialize<MessageModel>(m);
        try
        {
            _chatRepository.Add(m);
            var a = _chatRepository.GetUser(m.Username);
            if (a != null)
            {
                await Clients.Client(a.ConnectionId).SendAsync("Messaged", m);
            }
            var b = _chatRepository.GetUser(m.Send_Username);
            if (b != null)
            {
                await Clients.Client(b.ConnectionId).SendAsync("Messaged", m);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task Message(MessageModel message)
    {
        await Clients.Client(_chatRepository.Producer).SendAsync("Messaged", message);
    }
    public async Task Messages(string username)
    {
        var m = _chatRepository.GetMessages(username);
        await Clients.Client(Context.ConnectionId).SendAsync("Messaged", m);
    }
}
