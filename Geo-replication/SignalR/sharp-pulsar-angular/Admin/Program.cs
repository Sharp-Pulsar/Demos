// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using SharpPulsar.Admin.v2;

internal class Program
{
    static private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    static PulsarAdminRESTAPIClient _admin;
    static async Task Main(string[] args)
    {
        //https://pulsar.apache.org/docs/3.1.x/administration-geo/
        //https://pulsar.apache.org/docs/3.1.x/administration-geo/#enable-geo-replication-at-namespace-level
        //https://pulsar.apache.org/docs/3.1.x/admin-api-namespaces/#configure-replication-clusters
        //https://pulsar.apache.org/docs/3.1.x/concepts-replication/
        //https://marketplace.digitalocean.com/apps/apache-pulsar
        var client = new HttpClient
        {
            BaseAddress = new Uri("http://192.168.0.131:8080/admin/v2/")
        };
        _admin = new PulsarAdminRESTAPIClient(client);
        //await ReplicationClusters();
        //await Cluster();
        await Migration();
    }
    static async Task ReplicationClusters()
    {
        //persistent
        try
        {
            var cluster = await _admin.UpdateClusterAsync("cluster1", new ClusterData
            {
                BrokerServiceUrl = "pulsar://192.168.0.131:6650",
                ServiceUrl = "http://192.168.0.131:8080",
                ProxyProtocol = ClusterDataProxyProtocol.SNI,
                ProxyServiceUrl = "pulsar://192.168.0.131:6650",
                // org.apache.pulsar.broker.web.RestException: cluster2's peer-clusters [cluster2] can't be part of replication-clusters [cluster1, cluster2]
                //PeerClusterNames = new List<string> { "cluster", "cluster2", }
            });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(cluster, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }

        try
        {
            var cluster = await _admin.UpdateClusterAsync("cluster2", new ClusterData
            {
                BrokerServiceUrl = "pulsar://192.168.0.114:6650",
                ServiceUrl = "http://192.168.0.114:8080",
                ProxyProtocol = ClusterDataProxyProtocol.SNI,
                ProxyServiceUrl = "pulsar://192.168.0.114:6650",
                // org.apache.pulsar.broker.web.RestException: cluster2's peer-clusters [cluster2] can't be part of replication-clusters [cluster1, cluster2]
                //PeerClusterNames = new List<string> { "cluster", "cluster2", }
            });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(cluster, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }

        try
        {
            var tenant = await _admin.UpdateTenantAsync("public", new TenantInfo
            {
                AdminRoles = new[] { "admin" },
                AllowedClusters = new[] { "cluster1", "cluster2" },

            });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(tenant, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }            

        try
        {
            var setclusters = await _admin.SetNamespaceReplicationClustersAsync("public", "default", new List<string> { "cluster1", "cluster2" });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(setclusters, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine("#####################################################################################");
        }

    }
    static async Task Cluster()
    {
        try
        {
            var cluster = await _admin.UpdateClusterAsync("cluster1", new ClusterData
            {
                PeerClusterNames = new List<string> { "cluster1" , "cluster2" }
            });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(cluster, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }
    }
    static async Task Namespaces()
    {
        try
        {
            var name = await _admin.CreateNamespaceAsync("signalr", "default", new Policies
            {
                Replication_clusters = new[] { "cluster1", "cluster2" },

            });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(name, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }
        try
        {
            var name = await _admin.GrantPermissionOnNamespaceAsync("publice", "default", "admin", new List<Anonymous> { Anonymous.Produce, Anonymous.Consume });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(name, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }
    }

    static async Task Migration()
    {
        try
        {
            var cluster = await _admin.UpdateClusterAsync("lagos", new ClusterData
            {
                BrokerServiceUrl = "pulsar://192.168.0.131:6650",
                ServiceUrl = "http://192.168.0.131:8080",
                ProxyProtocol = ClusterDataProxyProtocol.SNI,
                ProxyServiceUrl = "pulsar://192.168.0.131:6650",
                // org.apache.pulsar.broker.web.RestException: cluster2's peer-clusters [cluster2] can't be part of replication-clusters [cluster1, cluster2]
                //PeerClusterNames = new List<string> { "cluster", "cluster2", }
            });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(cluster, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }
        try
        {
            var cluster = await _admin.CreateClusterAsync("ogun", new ClusterData
            {
                BrokerServiceUrl = "pulsar://192.168.0.114:6650",
                ServiceUrl = "http://192.168.0.114:8080",
                ProxyProtocol = ClusterDataProxyProtocol.SNI,
                ProxyServiceUrl = "pulsar://192.168.0.114:6650",
                // org.apache.pulsar.broker.web.RestException: cluster2's peer-clusters [cluster2] can't be part of replication-clusters [cluster1, cluster2]
                //PeerClusterNames = new List<string> { "cluster", "cluster2", }
            });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(cluster, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }

        try
        {
            var tenant = await _admin.UpdateTenantAsync("public", new TenantInfo
            {
                AdminRoles = new[] { "admin" },
                AllowedClusters = new[] { "lagos", "ogun" },

            });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(tenant, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());

            Console.WriteLine("#####################################################################################");
        }
        try
        {
            var setclusters = await _admin.SetNamespaceReplicationClustersAsync("public", "default", new List<string> { "lagos", "ogun" });
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(setclusters, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine("#####################################################################################");
        }
        try
        {
            var setclusters = await _admin.UnloadNamespaceAsync("public", "default");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(setclusters, _jsonSerializerOptions));
            Console.WriteLine("#####################################################################################");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine("#####################################################################################");
        }
    }
}


