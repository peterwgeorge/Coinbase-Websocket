namespace CryptoExchangeModels.Binance;

using Newtonsoft.Json;

public class BinanceResponse
{
    [JsonProperty("e")]
    public string? EventType { get; set; } // Event type

    [JsonProperty("E")]
    public long EventTime { get; set; } // Event time (Unix timestamp in ms)

    [JsonProperty("s")]
    public string? Symbol { get; set; } // Symbol (e.g., BTCUSDT)

    [JsonProperty("p")]
    public string? PriceChange { get; set; } // Price change (24h)

    [JsonProperty("P")]
    public string? PriceChangePercent { get; set; } // Price change percent (24h)

    [JsonProperty("w")]
    public string? WeightedAveragePrice { get; set; } // Weighted average price (24h)

    [JsonProperty("x")]
    public string? PreviousClosePrice { get; set; } // Previous day's close price

    [JsonProperty("c")]
    public string? LastPrice { get; set; } // Current day's close price (last price)

    [JsonProperty("Q")]
    public string? LastQuantity { get; set; } // Last quantity

    [JsonProperty("b")]
    public string? BestBidPrice { get; set; } // Best bid price

    [JsonProperty("B")]
    public string? BestBidQuantity { get; set; } // Best bid quantity

    [JsonProperty("a")]
    public string? BestAskPrice { get; set; } // Best ask price

    [JsonProperty("A")]
    public string? BestAskQuantity { get; set; } // Best ask quantity

    [JsonProperty("o")]
    public string? OpenPrice { get; set; } // Open price (24h)

    [JsonProperty("h")]
    public string? HighPrice { get; set; } // High price (24h)

    [JsonProperty("l")]
    public string? LowPrice { get; set; } // Low price (24h)

    [JsonProperty("v")]
    public string? BaseAssetVolume { get; set; } // Total traded base asset volume (24h)

    [JsonProperty("q")]
    public string? QuoteAssetVolume { get; set; } // Total traded quote asset volume (24h)

    [JsonProperty("O")]
    public long StatisticsOpenTime { get; set; } // Statistics open time (ms)

    [JsonProperty("C")]
    public long StatisticsCloseTime { get; set; } // Statistics close time (ms)

    [JsonProperty("F")]
    public long FirstTradeId { get; set; } // First trade ID

    [JsonProperty("L")]
    public long LastTradeId { get; set; } // Last trade ID

    [JsonProperty("n")]
    public long TotalTrades { get; set; } // Total number of trades
}
