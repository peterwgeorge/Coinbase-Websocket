namespace CryptoExchangeModels.Common;

using CryptoExchangeModels.Binance;
using CryptoExchangeModels.Coinbase;
using CryptoExchangeModels.Kraken;

public static class FeedFactory
{
    public static string GetExchangeFeed(string exchange)
    {
        switch(exchange.ToLower()){
            case "coinbase":
                return CoinbaseMarketDataFeeds.MarketDataEndpoint;
            case "kraken":
                return KrakenMarketDataFeeds.Endpoint;
            case "binance":
                return BinanceMarketDataFeeds.BaseEndpoint;
            default:
                throw new ArgumentException($"{exchange} not supported.");
        }
    }
}