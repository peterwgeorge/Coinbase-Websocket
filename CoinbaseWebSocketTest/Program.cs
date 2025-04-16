using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;
using CoinbaseWebSocketConstructs;
using AmazonSecretsManagerHandler;

class Program
{  
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        SecretsProvider.Configure(configuration);

        using (var socket = new ClientWebSocket())
        {
            try{
                await SecretsProvider.Initialize();
                await ConnectionHandler.Connect(socket, CoinbaseMarketDataFeeds.MarketDataEndpoint);
                await ConnectionHandler.Subscribe(socket, CoinbaseChannelNames.Ticker, ["BTC-USD"]);
            
                byte[] buffer = new byte[4096];
                while (socket.State == WebSocketState.Open)
                {
                    await ConnectionHandler.Receive(socket, buffer);
                }
            }
            catch (WebSocketException ex){
                 Console.WriteLine("WebSocket error: " + ex.Message);
            }
        }
    }
}
