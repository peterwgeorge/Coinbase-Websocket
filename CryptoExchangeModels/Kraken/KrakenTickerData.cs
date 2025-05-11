namespace CryptoExchangeModels.Kraken;

using Newtonsoft.Json;
public class KrakenTickerData
{
    [JsonProperty("symbol")]
    public string? Symbol { get; set; }

    [JsonProperty("bid")]
    public decimal Bid { get; set; }

    [JsonProperty("bid_qty")]
    public decimal BidQty { get; set; }

    [JsonProperty("ask")]
    public decimal Ask { get; set; }

    [JsonProperty("ask_qty")]
    public decimal AskQty { get; set; }

    [JsonProperty("last")]
    public decimal Last { get; set; }

    [JsonProperty("volume")]
    public decimal Volume { get; set; }

    [JsonProperty("vwap")]
    public decimal Vwap { get; set; }

    [JsonProperty("low")]
    public decimal Low { get; set; }

    [JsonProperty("high")]
    public decimal High { get; set; }

    [JsonProperty("change")]
    public decimal Change { get; set; }

    [JsonProperty("change_pct")]
    public decimal ChangePct { get; set; }
}