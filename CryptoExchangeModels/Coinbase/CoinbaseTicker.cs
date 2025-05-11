namespace CryptoExchangeModels.Coinbase;

using Newtonsoft.Json;

public class CoinbaseTicker
{
    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("product_id")]
    public string? ProductId { get; set; }

    [JsonProperty("price")]
    public string? Price { get; set; }

    [JsonProperty("volume_24_h")]
    public string? Volume24H { get; set; }

    [JsonProperty("low_24_h")]
    public string? Low24H { get; set; }

    [JsonProperty("high_24_h")]
    public string? High24H { get; set; }

    [JsonProperty("low_52_w")]
    public string? Low52W { get; set; }

    [JsonProperty("high_52_w")]
    public string? High52W { get; set; }

    [JsonProperty("price_percent_chg_24_h")]
    public string? PricePercentChg24H { get; set; }

    [JsonProperty("best_bid")]
    public string? BestBid { get; set; }

    [JsonProperty("best_ask")]
    public string? BestAsk { get; set; }

    [JsonProperty("best_bid_quantity")]
    public string? BestBidQuantity { get; set; }

    [JsonProperty("best_ask_quantity")]
    public string? BestAskQuantity { get; set; }
}