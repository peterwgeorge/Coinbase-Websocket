namespace CryptoExchangeModels.Kraken;

using Newtonsoft.Json;
using System.Collections.Generic;

public class KrakenResponse
{
    [JsonProperty("channel")]
    public string? Channel { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("data")]
    public List<KrakenTickerData>? Data { get; set; }
}
