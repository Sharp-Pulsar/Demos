using SharpPulsar;
using System.Text.Json;
using Shared.Models;

namespace Shared.SharpPulsarClient
{
    public class SignalRPulsar
    {
        private string _url = "pulsar://lagos-pulsar-broker-0.lagos-pulsar-broker.lagos.svc.cluster.local:6650";
        private Consumer<Post> _cp;
        private Consumer<Client> _cu;
        private Consumer<MessageModel> _cm;

        private Reader<Post> _rp;
        private Reader<Client> _ru;
        private Reader<MessageModel> _rm;

        private Producer<Post> _pp;
        private Producer<Client> _pu;
        private Producer<MessageModel> _pm;
        private int _postSequenceId = 0;
        private int _msgSequenceId = 0;
        private int _userSequenceId = 0;
        private PulsarClient _client;
        private WebPulsarClient _web;
        public SignalRPulsar()
        {
            _web = new WebPulsarClient();
        }
        public async Task Connect()
        {
            _client = await _web.Connect(_url,"pulsar://192.168.0.131:6650");            
        }

        public async Task Post()
        {
            _pp = await _web.Post(_client, $"post-{Guid.NewGuid()}");
        }
        public async Task ConsumerPost()
        {
            _cp = await _web.ConsumerPost(_client, $"post-subscriber");
        }
        public async Task ReaderPost()
        {
            _rp = await _web.ReaderPost(_client, $"reader-sub-post");
        }
        public async Task Message()
        {
            _pm = await _web.Message(_client, $"message-{Guid.NewGuid()}");
        }
        public async Task ConsumerMessage()
        {
            _cm = await _web.ConsumerMessage(_client, $"message-subscriber");
        }
        public async Task ReaderMessage()
        {
            _rm = await _web.ReaderMessage(_client, $"reader-sub-message");
        }
        public async Task Username()
        {
            _pu = await _web.Username(_client, $"username-{Guid.NewGuid()}");
        }
        public async Task ConsumerClient()
        {
            _cu = await _web.ConsumerClient(_client, $"username-subscriber");
        }
        public async Task ReaderClient()
        {
            _ru = await _web.ReaderClient(_client, $"reader-sub-username");
        }
        public async Task<string> Post(Post post)
        {
            try
            {
                //.ReplicationClusters(new List<string> { "cluster2" })
                var p = await _pp.NewMessage().SequenceId(_postSequenceId).Value(post).SendAsync();
                _postSequenceId++;
                var s = JsonSerializer.Serialize(p, new JsonSerializerOptions { WriteIndented = true });
                return s;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> Message(MessageModel message)
        {
            try
            {
                //.ReplicationClusters(new List<string> { "cluster2" })
                var p = await _pm.NewMessage().SequenceId(_msgSequenceId).Value(message).SendAsync();
                _msgSequenceId++;
                var s = JsonSerializer.Serialize(p, new JsonSerializerOptions { WriteIndented = true });
                return s;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<string> Username(Client username)
        {
            try
            {
                //.ReplicationClusters(new List<string> { "cluster2" })
                var p = await _pu.NewMessage().SequenceId(_userSequenceId).Value(username).SendAsync();
                _userSequenceId++;
                var s = JsonSerializer.Serialize(p, new JsonSerializerOptions { WriteIndented = true });
                return s;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async Task<Message<Post>?> PostConsumers()
        {
            Message<Post>? po = null;
            var post = await _cp.ReceiveAsync(TimeSpan.FromSeconds(10));

            if (post != null)
            {
                await _cp.AcknowledgeAsync(post);
                po = (Message<Post>)post;
            }
            return po;
        }

        public async Task<Message<MessageModel>?> MessageConsumers()
        {
            Message<MessageModel>? m = null;

            var message = await _cm.ReceiveAsync(TimeSpan.FromSeconds(10));
            if (message != null)
            {
                await _cm.AcknowledgeAsync(message);
                m = (Message<MessageModel>)message;
            }
            return m;
        }
        public async Task<Message<Client>?> UsernameConsumers()
        {
            Message<Client>? m = null;

            var message = await _cu.ReceiveAsync(TimeSpan.FromSeconds(10));
            if (message != null)
            {
                await _cu.AcknowledgeAsync(message);
                m = (Message<Client>)message;
            }
            return m;
        }
        public async Task<Message<Client>?> UsernameReaders()
        {
            var username = await _ru.ReadNextAsync(TimeSpan.FromSeconds(1));
            if (username != null)
            {
                return (Message<Client>)username;
            }
            return null;
        }
        public async Task<Message<Post>?> PostReaders()
        {
            var post = await _rp.ReadNextAsync(TimeSpan.FromSeconds(1));

            if (post != null)
            {
                return (Message<Post>)post;
            }

            return null;
        }
        public async Task<Message<MessageModel>?> MessageReaders()
        {
            var message = await _rm.ReadNextAsync(TimeSpan.FromSeconds(1)); ;
            if (message != null)
            {
                return (Message<MessageModel>)message;
            }
            return null;
        }
        
    }
}
