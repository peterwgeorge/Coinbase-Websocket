namespace WebsocketServer.ConnectionHandlers;

using System.Text;
using System.Net.WebSockets;
using Newtonsoft.Json;
using CryptoExchangeModels.Common;
using Normalization.Parse;
using Normalization.Types;

public class ConnectionHandler{
    
    public ClientWebSocket Socket;
    public string Exchange;

    public ConnectionHandler(ClientWebSocket socket, string exchange)
    {
        Socket = socket;
        Exchange = exchange;
    }
    
    public async Task Connect(){
        string feed = FeedFactory.GetExchangeFeed(Exchange);
        Uri uri = new Uri(feed);
        await Socket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine($"Connected to {Exchange} WebSocket feed {feed}.");
    }

    public async Task Subscribe(string channel, string[] symbols)
    {
        IExchangeRequest request = RequestFactory.CreateSubscribeRequest(Exchange, channel, symbols);
        string json = JsonConvert.SerializeObject(request);
        Console.WriteLine($"Sending {Exchange} request message:");
        Console.WriteLine(json);

        var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
        await Socket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task Receive(byte[] buffer)
    {
        var result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine($"{Exchange} WebSocket closed.");
            await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
        else
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received from {Exchange}: " + message);
            PricePoint p = ExchangeDataParser.GetPricePoint(message);
            if(p.Exchange == "unknown")
                return;

            await RelayServer.BroadcastToClientsAsync(JsonConvert.SerializeObject(p));
        }
    }
}