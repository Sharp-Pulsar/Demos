// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Models;
using Shared.SharpPulsarClient;

internal class Program
{
    private static HubConnection _connection;
    static SignalRPulsar _pulsar = new SignalRPulsar();
    static async Task Main(string[] args)
    {
        _connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.0.131:7000/chathub?username=producer&password=producer", HttpTransportType.LongPolling)
                .WithAutomaticReconnect()
                .Build();
        await _pulsar.Connect();
        await _pulsar.Post();
        await _pulsar.Message();
        await _pulsar.Username();
        Console.WriteLine("JOINED");
        _connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await _connection.StartAsync();
        };
        _connection.On<Post>("Posted", async posted => 
        {
            try
            {
                Console.WriteLine(posted.ToString());
                var p = await _pulsar.Post(posted);
                await _connection.InvokeAsync("Logged", p);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await _connection.InvokeAsync("Logged", ex.ToString());
            }
        });
        _connection.On<MessageModel>("Messaged", async messaged =>
        {
            try
            {
                var p = await _pulsar.Message(messaged);
                await _connection.InvokeAsync("Logged", p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _connection.InvokeAsync("Logged", ex.ToString());
            }
        });
        _connection.On<Client>("Usernamed", async username =>
        {
            try
            {
                var p = await _pulsar.Username(username);
                await _connection.InvokeAsync("Logged", p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _connection.InvokeAsync("Logged", ex.ToString());
            }
        });
        await _connection.StartAsync();
        Console.ReadKey();
    }
    
}  
