using System.Net.WebSockets;
using CoinbaseWebSocketConstructs;
using AmazonSecretsManagerHandler;
using WebsocketServer.ConnectionHandlers;
using BinanceWebSocketConstructs;
using KrakenWebSocketConstructs;

class Program
{  
    static async Task Main(string[] args)
    {
        using (var socket = new ClientWebSocket())
        {
            try{
                await KrakenConnectionHandler.Connect(socket, KrakenMarketDataFeeds.Endpoint);
                await KrakenConnectionHandler.Subscribe(socket, "ticker", ["BTC/USD"]);
            
                byte[] buffer = new byte[4096];
                while (socket.State == WebSocketState.Open)
                {
                    await KrakenConnectionHandler.Receive(socket, buffer);
                }
            }
            catch (WebSocketException ex){
                 Console.WriteLine("WebSocket error: " + ex.Message);
            }
        }
    }
}