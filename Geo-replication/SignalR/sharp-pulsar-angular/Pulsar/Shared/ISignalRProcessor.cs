using Shared.Models;

namespace Shared.SignalRHubs
{
    public interface ISignalRProcessor
    {
        Task Post(Post post);
        Task Message(MessageModel message);
        Task Username(Client client);
    }
}
