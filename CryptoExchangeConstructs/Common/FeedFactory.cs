namespace CryptoExchangeConstructs.Common;

using CryptoExchangeConstructs.Binance;
using CryptoExchangeConstructs.Coinbase;
using CryptoExchangeConstructs.Kraken;

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