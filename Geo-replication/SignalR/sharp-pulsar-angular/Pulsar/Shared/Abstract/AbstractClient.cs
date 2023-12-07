
using SharpPulsar;
using SharpPulsar.Builder;
using SharpPulsar.Common;
using SharpPulsar.Interfaces;
using SharpPulsar.Schemas;
using Shared.Interfaces;
using Shared.Models;
// https://medium.com/capital-one-tech/apache-pulsar-geo-replication-and-hybrid-deployment-model-to-achieve-synchronous-replication-35f30e8b0f2
namespace Shared.Abstract
{
    internal abstract class AbstractClient: IClient<string, string, PulsarClient>
    {
        protected readonly AvroSchema<Post> _schemaPost = AvroSchema<Post>.Of(typeof(Post));
        protected readonly AvroSchema<Client> _schemaClient = AvroSchema<Client>.Of(typeof(Client));
        protected readonly AvroSchema<MessageModel> _schemaMessage = AvroSchema<MessageModel>.Of(typeof(MessageModel));
        public virtual async ValueTask<PulsarClient> Connect(string service, string proxy)
        {
            var clientConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(service)
                
                //.ListenerName("external");
                .ProxyServiceUrl(proxy, ProxyProtocol.SNI);
            var pulsarSystem = PulsarSystem.GetInstance(actorSystemName: "web");
            var client = await pulsarSystem.NewClient(clientConfig).ConfigureAwait(false);
            return client;
        }
        public virtual async ValueTask<PulsarClient> Connect(string service)
        {
            var clientConfig = new PulsarClientConfigBuilder()
                .ServiceUrl(service);
            var pulsarSystem = PulsarSystem.GetInstance(actorSystemName: "web");
            var client = await pulsarSystem.NewClient(clientConfig).ConfigureAwait(false);
            return client;
        }
        public virtual async ValueTask<Producer<Post>> Post(PulsarClient client, string name, string topic = "persistent://public/default/post")
        {
            var producerConfig = new ProducerConfigBuilder<Post>()
                .ProducerName(name)
                .Topic(topic)
                .Schema(_schemaPost);

            return await client.NewProducerAsync(_schemaPost, producerConfig).ConfigureAwait(false);
        }
        public virtual async ValueTask<Producer<MessageModel>> Message(PulsarClient client, string name, string topic = "persistent://public/default/message")
        {
            var producerConfig = new ProducerConfigBuilder<MessageModel>()
                .ProducerName(name)
                .Topic(topic)                
                .Schema(_schemaMessage);

            return await client.NewProducerAsync(_schemaMessage, producerConfig).ConfigureAwait(false);
        }
        public virtual async ValueTask<Consumer<Post>> ConsumerPost(PulsarClient client, string name, string topic = "persistent://public/default/post")
        {
            var postConsumer = new ConsumerConfigBuilder<Post>()
                .Topic(topic)
                .SubscriptionName(name)
                .IsAckReceiptEnabled(true)
                .ReplicateSubscriptionState(true)
                .ForceTopicCreation(true);

            return await client.NewConsumerAsync(_schemaPost, postConsumer).ConfigureAwait(false);
        }
        public virtual async ValueTask<Consumer<MessageModel>> ConsumerMessage(PulsarClient client, string name, string topic = "persistent://public/default/message")
        {
            var postConsumer = new ConsumerConfigBuilder<MessageModel>()
                .Topic(topic)
                .SubscriptionName(name)
                .IsAckReceiptEnabled(true)
                .ReplicateSubscriptionState(true)
                .ForceTopicCreation(true);

            return await client.NewConsumerAsync(_schemaMessage, postConsumer).ConfigureAwait(false);
        }
        public virtual async ValueTask<Reader<Post>> ReaderPost(PulsarClient client, string name, string topic = "persistent://public/default/post")
        {
            var postReader = new ReaderConfigBuilder<Post>()
                .Topic(topic)
                .StartMessageId(IMessageId.Earliest)                
                .ReaderName(name);
            return await client.NewReaderAsync(_schemaPost, postReader).ConfigureAwait(false);
        }
        public virtual async ValueTask<Reader<MessageModel>> ReaderMessage(PulsarClient client, string name, string topic = "persistent://public/default/message")
        {
            var postReader = new ReaderConfigBuilder<MessageModel>()
                .Topic(topic)
                .StartMessageId(IMessageId.Earliest)
                .ReaderName(name);
            return await client.NewReaderAsync(_schemaMessage, postReader).ConfigureAwait(false);
        }
        public virtual async ValueTask<Producer<Client>> Username(PulsarClient client, string name, string topic = "persistent://public/default/username")
        {
            var producerConfig = new ProducerConfigBuilder<Client>()
                .ProducerName(name)
                .Topic(topic)
                .Schema(_schemaClient);

            return await client.NewProducerAsync(_schemaClient, producerConfig).ConfigureAwait(false);
        }
        public virtual async ValueTask<Consumer<Client>> ConsumerClient(PulsarClient client, string name, string topic = "persistent://public/default/username")
        {
            var username = new ConsumerConfigBuilder<Client>()
                .Topic(topic)
                .SubscriptionName(name)
                .IsAckReceiptEnabled(true)
                .ReplicateSubscriptionState(true)
                .ForceTopicCreation(true);

            return await client.NewConsumerAsync(_schemaClient, username).ConfigureAwait(false);;
        }
        public virtual async ValueTask<Reader<Client>> ReaderClient(PulsarClient client, string name, string topic = "persistent://public/default/username")
        {
            var username = new ReaderConfigBuilder<Client>()
                .Topic(topic)
                .StartMessageId(IMessageId.Earliest)
                .ReaderName(name); 

            return await client.NewReaderAsync(_schemaClient, username).ConfigureAwait(false); 
        }
    }
}
