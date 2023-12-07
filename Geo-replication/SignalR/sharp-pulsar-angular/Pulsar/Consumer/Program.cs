// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Models;
using Shared.SharpPulsarClient;
using System.Text.Json;
using static Akka.Actor.ProviderSelection;

internal class Program
{
    private static HubConnection _connection;
    static SignalRPulsar _pulsar = new SignalRPulsar();
    static private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    static async Task Main(string[] args)
    {
        _connection = new HubConnectionBuilder()
                    .WithUrl("http://192.168.0.131:7000/chathub?username=consumer&password=consumer", HttpTransportType.LongPolling)
                    .WithAutomaticReconnect()
                    .Build();
        await _pulsar.Connect();
        await _pulsar.ConsumerPost();
        await _pulsar.ConsumerMessage();
        //await _pulsar.ReaderPost();
        //await _pulsar.ReaderMessage();
        await _pulsar.ConsumerClient();
        //await _pulsar.ReaderClient();
        _connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await _connection.StartAsync();
        };
        await _connection.StartAsync();
        Console.WriteLine("TRUST");
        while (true) 
        {
            //Console.WriteLine("TRUST");
            try
            {
                var p = await _pulsar.UsernameConsumers();
                if (p != null && p.Value.Username != null)
                {
                    var msg = p.Value;
                    var l = $"USERNAMED SequenceId: {p.SequenceId} - {p.Topic}";
                    await _connection.InvokeAsync("Logged", l);
                    Console.WriteLine(l);
                    await _connection.InvokeAsync("Logged", l);
                    Console.WriteLine(JsonSerializer.Serialize(msg, _jsonSerializerOptions));
                    await _connection.InvokeAsync("ConsumerUsername", msg);
                }
                
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.ToString());
            }
            try
            {
                var p = await _pulsar.PostConsumers();
                if (p != null && p.Value.Username != null)
                {
                    var msg = p.Value;
                    var l = $"POSTED SequenceId: {p.SequenceId} - {p.Topic}";
                    await _connection.InvokeAsync("Logged", l);
                    Console.WriteLine(l);
                    await _connection.InvokeAsync("Logged", l);
                    Console.WriteLine(JsonSerializer.Serialize(msg, _jsonSerializerOptions));
                    await _connection.InvokeAsync("ConsumerPost", msg);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            try
            {
                var m = await _pulsar.MessageConsumers();
                if (m != null && m.Value.Username != null)
                {
                    var msg = m.Value;
                    var l = $"MESSAGED SequenceId: {m.SequenceId} - {m.Topic}";
                    await _connection.InvokeAsync("Logged", l);
                    Console.WriteLine(l);
                    Console.WriteLine(JsonSerializer.Serialize(msg, _jsonSerializerOptions));
                    await _connection.InvokeAsync("ConsumerMessage", msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            await Task.Delay(1000);
        };
    }
}