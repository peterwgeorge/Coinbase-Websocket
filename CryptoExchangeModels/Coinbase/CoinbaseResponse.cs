namespace CryptoExchangeModels.Coinbase;

using Newtonsoft.Json;

public class CoinbaseResponse
{
    [JsonProperty("channel")]
    public string? Channel { get; set; }

    [JsonProperty("client_id")]
    public string? ClientId { get; set; }

    [JsonProperty("timestamp")]
    public string? Timestamp { get; set; }

    [JsonProperty("sequence_num")]
    public int SequenceNum { get; set; }

    [JsonProperty("events")]
    public List<TickerEvent>? Events { get; set; }
}
