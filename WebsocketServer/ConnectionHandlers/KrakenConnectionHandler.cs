namespace WebsocketServer.ConnectionHandlers;

using System.Text;
using System.Net.WebSockets;
using Newtonsoft.Json;
using CryptoExchangeConstructs.Kraken;

public static class KrakenConnectionHandler{
    public static async Task Connect(ClientWebSocket socket, string feed){
        Uri uri = new Uri(feed);
        await socket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine($"Connected to Kraken WebSocket feed {feed}.");
    }

    public static async Task Subscribe(ClientWebSocket socket, string channel, string[] product_ids)
    {
        KrakenRequest request = new(KrakenMethodTypes.Subscribe, product_ids, channel);
        string json = JsonConvert.SerializeObject(request);
        Console.WriteLine("Sending Kraken request message:");
        Console.WriteLine(json);

        var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
        await socket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public static async Task Receive(ClientWebSocket socket, byte[] buffer)
    {
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine("Kraken WebSocket closed.");
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
        else
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine("Received from Kraken: " + message);

            await RelayServer.BroadcastToClientsAsync(message);
        }
    }
}