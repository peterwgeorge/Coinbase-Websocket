using System.Text;
using System.Net.WebSockets;
using Newtonsoft.Json;

public static class BinanceConnectionHandler{
    public static async Task Connect(ClientWebSocket socket, string feed){
        Uri uri = new Uri(feed);
        await socket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine($"Connected to Binance WebSocket feed {feed}.");
    }

    public static async Task Subscribe(ClientWebSocket socket, string channel, string[] product_ids)
    {
        Func<Task> task = async () => {
            await Task.Delay(500);
            Console.WriteLine("Subscribe not implemented yet.");
        };

        await task();
    }

    public static async Task Receive(ClientWebSocket socket, byte[] buffer)
    {
        Func<Task> task = async () => {
            await Task.Delay(500);
            Console.WriteLine("Receive not implemented yet.");
        };
        
        await task();
    }
}