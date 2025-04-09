using System.Net.WebSockets;
using CoinbaseWebSocketConstructs;
using AmazonSecretsManagerModels;

class Program
{  
    static async Task Main(string[] args)
    {
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
