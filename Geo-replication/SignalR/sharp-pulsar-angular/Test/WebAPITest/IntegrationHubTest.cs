using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using System.Text.Json;
using webapi;
using WebAPITest.Common;
using Xunit.Abstractions;

namespace WebAPITest
{
    public class IntegrationHubTest
    {
        private readonly TestServer _server = new();
        private readonly ITestOutputHelper _output;

        public IntegrationHubTest(ITestOutputHelper output)
        {
            _output = output;

            _server.StartAsync().GetAwaiter();
        }

        [Fact]
        public async Task WebAPI_GetTests()
        {
            var apiResponse = await _server.WebApiClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("/api/forecast");
            _output.WriteLine(JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions { WriteIndented = true }));
            Assert.NotNull(apiResponse);
        }
        [Fact]
        public async Task SignalR_PostTests()
        {
            _server.WebSocketClient.On<string>("Sent", s =>
            {
                _output.WriteLine(s);

                Assert.NotNull(s);
            });
            for (var i = 0; i < 100; i++)
                await _server.WebSocketClient.InvokeAsync("Message", "Ebere " + i);
        }
        [Fact]
        public void Test1()
        {

        }
    }
}