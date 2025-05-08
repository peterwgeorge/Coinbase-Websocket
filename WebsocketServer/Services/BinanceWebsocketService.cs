using CryptoExchangeConstructs.Binance;
using WebsocketServer.ConnectionHandlers;
using System.Net.WebSockets;

public class BinanceWebSocketService : BackgroundService
{
    private readonly ILogger<BinanceWebSocketService> _logger;
    private readonly IConfiguration _configuration;

    public BinanceWebSocketService(ILogger<BinanceWebSocketService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Binance WebSocket service");
        using var socket = new ClientWebSocket();
        try
        {
            await BinanceConnectionHandler.Connect(socket, BinanceMarketDataFeeds.BaseEndpoint);
            await BinanceConnectionHandler.Subscribe(socket, BinanceMarketDataMethodTypes.Subscribe, ["btcusdt@ticker"]);

            byte[] buffer = new byte[4096];
            
            while (!stoppingToken.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                await BinanceConnectionHandler.Receive(socket, buffer);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred in the Binance WebSocket service");
        }
        finally
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Binance Service stopping", stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error closing Binance WebSocket connection");
                }
            }
        }
    }
}
