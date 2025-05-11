namespace CryptoExchangeModels.Coinbase;

using Newtonsoft.Json;

public class TickerEvent
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("tickers")]
    public List<CoinbaseTicker>? Tickers { get; set; }
}