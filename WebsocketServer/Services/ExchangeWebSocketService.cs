using AmazonSecretsManagerHandler;
using CryptoExchangeModels.Coinbase;
using WebsocketServer.ConnectionHandlers;
using System.Net.WebSockets;

public class ExchangeWebSocketService : BackgroundService
{
    private readonly ILogger<ExchangeWebSocketService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _exchange;
    private readonly string _channel;
    private readonly string[] _symbols;

    public ExchangeWebSocketService(
        ILogger<ExchangeWebSocketService> logger,
        IConfiguration configuration,
        string exchange,
        string channel,
        string[] symbols)
    {
        _logger = logger;
        _configuration = configuration;
        _exchange = exchange;
        _channel = channel;
        _symbols = symbols;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting {_exchange} WebSocket service");
        using var socket = new ClientWebSocket();
        var handler = new ConnectionHandler(socket, _exchange);

        try
        {
            if(_exchange == "coinbase"){
                SecretsProvider.Configure(_configuration);
                await SecretsProvider.Initialize();
            }
            
            await handler.Connect();
            await handler.Subscribe(_channel, _symbols);

            byte[] buffer = new byte[4096];

            while (!stoppingToken.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                await handler.Receive(buffer);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error occurred in the {_exchange} WebSocket service");
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
                    _logger.LogWarning(ex, $"Error closing {_exchange} WebSocket connection");
                }
            }
        }
    }
}

