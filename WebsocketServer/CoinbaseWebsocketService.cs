using AmazonSecretsManagerHandler;
using CoinbaseWebSocketConstructs;
using System.Net.WebSockets;

public class CoinbaseWebSocketService : BackgroundService
{
    private readonly ILogger<CoinbaseWebSocketService> _logger;
    private readonly IConfiguration _configuration;

    public CoinbaseWebSocketService(ILogger<CoinbaseWebSocketService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SecretsProvider.Configure(_configuration);
        await SecretsProvider.Initialize();
        
        _logger.LogInformation("Starting Coinbase WebSocket service");
        
        using var socket = new ClientWebSocket();
        try
        {
            await CoinbaseConnectionHandler.Connect(socket, CoinbaseMarketDataFeeds.MarketDataEndpoint);
            await CoinbaseConnectionHandler.Subscribe(socket, CoinbaseChannelNames.Ticker, ["BTC-USD"]);
            
            byte[] buffer = new byte[4096];
            
            while (!stoppingToken.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                await CoinbaseConnectionHandler.Receive(socket, buffer);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred in the Coinbase WebSocket service");
        }
        finally
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Service stopping", stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error closing WebSocket connection");
                }
            }
        }
    }
}
