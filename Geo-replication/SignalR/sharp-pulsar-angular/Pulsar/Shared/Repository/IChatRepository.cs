namespace Shared.Models;

public interface IChatRepository
{
    void Add(Client client);
    void Add(string log);
    void Add(Post post);
    void Add(MessageModel message);
    Client Get(string connectionId);
    string Producer { get; set; }
    string Consumer { get; set; }
    IEnumerable<string> StringGet();
    Client GetUser(string username);

    IEnumerable<Client> Get();

    IEnumerable<Client> GetExcept(string connectionId);
    IEnumerable<Client> GetUserExcept(string username);
    IEnumerable<Post> GetPosts();
    IEnumerable<MessageModel> GetMessages(string username);

    Client Remove(string connectionId, out IEnumerable<Client> clients);
}
