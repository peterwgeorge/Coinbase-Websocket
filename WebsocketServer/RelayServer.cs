    using System;
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using System.Text;
    using Microsoft.AspNetCore.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Hosting;

    public class RelayServer
    {
        private static readonly ConcurrentDictionary<string, WebSocket> _clients = new ConcurrentDictionary<string, WebSocket>();
        
        private readonly IConfiguration _configuration;

        public RelayServer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static async Task BroadcastToClientsAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(buffer);
            
            foreach (var client in _clients)
            {
                if (client.Value.State == WebSocketState.Open)
                {
                    try
                    {
                        await client.Value.SendAsync(
                            arraySegment,
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending to client {client.Key}: {ex.Message}");
                        _clients.TryRemove(client.Key, out _);
                    }
                }
                else
                {
                    _clients.TryRemove(client.Key, out _);
                }
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddWebSockets(options => {
                options.KeepAliveInterval = TimeSpan.FromMinutes(2);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Configure CORS for React app
            app.UseCors(builder => builder
                .WithOrigins(_configuration["ReactAppUrl"]) // React app URL
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            app.UseWebSockets();

            // Handle WebSocket connections
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var clientId = Guid.NewGuid().ToString();
                        _clients.TryAdd(clientId, webSocket);

                        // Keep the socket open
                        await HandleWebSocketConnection(webSocket, clientId);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket, string clientId)
        {
            var buffer = new byte[4096];
            
            var welcomeMessage = Encoding.UTF8.GetBytes("Connected to Coinbase WebSocket relay");
            await webSocket.SendAsync(
                new ArraySegment<byte>(welcomeMessage),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Client closed connection",
                            CancellationToken.None);
                            
                        _clients.TryRemove(clientId, out _);
                    }
                }
                catch
                {
                    _clients.TryRemove(clientId, out _);
                    break;
                }
            }
        }
    }
