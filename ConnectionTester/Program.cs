using System.Net.WebSockets;
using CoinbaseWebSocketConstructs;
using AmazonSecretsManagerHandler;
using WebsocketServer.ConnectionHandlers;
using BinanceWebSocketConstructs;

class Program
{  
    static async Task Main(string[] args)
    {
        using (var socket = new ClientWebSocket())
        {
            try{
                await SecretsProvider.Initialize();
                await BinanceConnectionHandler.Connect(socket, BinanceMarketDataFeeds.BaseEndpoint);
                await BinanceConnectionHandler.Subscribe(socket, BinanceMarketDataMethodTypes.Subscribe, ["btcusdt@ticker"]);
            
                byte[] buffer = new byte[4096];
                while (socket.State == WebSocketState.Open)
                {
                    await BinanceConnectionHandler.Receive(socket, buffer);
                }
            }
            catch (WebSocketException ex){
                 Console.WriteLine("WebSocket error: " + ex.Message);
            }
        }
    }
}