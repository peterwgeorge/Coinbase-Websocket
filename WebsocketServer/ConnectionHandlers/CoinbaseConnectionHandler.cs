using System.Text;
using System.Net.WebSockets;
using Newtonsoft.Json;
using CryptoExchangeConstructs.Coinbase;

public static class CoinbaseConnectionHandler{
    public static async Task Connect(ClientWebSocket socket, string feed){
        Uri uri = new Uri(feed);
        await socket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine($"Connected to Coinbase WebSocket feed {feed}.");
    }

    public static async Task Subscribe(ClientWebSocket socket, string channel, string[] product_ids)
    {
        CoinbaseRequest subscribe = new(CoinbaseMessageTypes.Subscribe, channel, product_ids);
        string json = JsonConvert.SerializeObject(subscribe);
        Console.WriteLine("Sending auth subscribe message to Coinbase:");
        Console.WriteLine(json);

        var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
        await socket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public static async Task Receive(ClientWebSocket socket, byte[] buffer)
    {
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine("Coinbase WebSocket closed.");
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
        else
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine("Received from Coinbase: " + message);

             await RelayServer.BroadcastToClientsAsync(message);
        }
    }
}