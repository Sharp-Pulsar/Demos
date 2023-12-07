# Apache Pulsar Geo-replicaton
![Screenshot](/image/replication.webp)
- [x] https://pulsar.apache.org/docs/3.1.x/administration-geo/
- [x] https://streamnative.io/blog/migrating-tenants-across-clusters-with-pulsars-geo-replication
- [x] https://www.youtube.com/watch?v=x7yBJT6_yBA
- [x] https://learn.microsoft.com/en-us/azure/azure-signalr/howto-enable-geo- 

# Use Case
1. Login
2. Post Text
3. Direct Message

# EventModeling
![Screenshot](/image/Pulsar.jpg)

# Installation KinD
On Windows:

```powershell
curl.exe -Lo kind-windows-amd64.exe https://kind.sigs.k8s.io/dl/v0.20.0/kind-windows-amd64
Move-Item .\kind-windows-amd64.exe c:\some-dir-in-your-PATH\kind.exe

# OR via Chocolatey (https://chocolatey.org/packages/kind)
choco install kind
```

To use kind, you will need to [install docker].
Once you have docker running you can create a cluster with:

```console
kind create cluster
```
# Build steps
1. Apache Pulsar
   - docker pull apachepulsar/pulsar-all:3.1.1
   
2. Helm
   - choco install kubernetes-helm
   - helm repo add apache https://pulsar.apache.org/charts
   - helm repo update

3. angular
   - docker build -t angularapp:v1.0.0 -f Dockerfile . # ./Demos/Geo-replication/SignalR/sharp-pulsar-angular/angularapp
   - docker run --name angular -p ***.***.***.***:8010:80 -p ***.***.***.***:8020:443 -d angularapp:v1.0.0 
   
4. webapi
   - docker build -t webapi:v1.0.0 -f ./webapi/Dockerfile .
   - docker run --name webapi -p ***.***.***.***:7000:80 -p ***.***.***.***:7001:443 -d webapi:v1.0.0 
   
# Running with Docker
1. Global ZooKeeper
   - kubectl create namespace zookeeper
   - helm install global -n zookeeper -f zoo_global.yaml apache/pulsar   
   - kubectl port-forward --address localhost,***.***.***.*** --namespace zookeeper global-pulsar-zookeeper-0 8000:8000 2181:2181 2888:2888 3888:3888
2. Regions
   - Lagos
     - Windows 11
     - WIFI: ***.***.***.***
	 - kubectl create namespace lagos
	 - helm install lagos -n lagos -f value.yaml apache/pulsar
	 - kubectl port-forward --address localhost,***.***.***.*** --namespace lagos lagos-pulsar-proxy-0 8080:8080 6650:6650 
   - Ogun
     - Windows 10
     - WIFI: ***.***.***.***
	 - kubectl create namespace ogun
	 - helm install ogun -n ogun -f value.yaml apache/pulsar
	 - kubectl port-forward --address localhost,***.***.***.*** --namespace ogun ogun-pulsar-proxy-0 8080:8080 6650:6650 
3. Admin
   - UpdateClusterAsync and/or CreateClusterAsync
   - UpdateTenantAsync
   - SetNamespaceReplicationClustersAsync
   - UnloadNamespaceAsync

