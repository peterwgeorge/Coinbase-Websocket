namespace Normalization.Types;

using Newtonsoft.Json;

public class PricePoint{
    [JsonProperty(PropertyName = "timestamp")]
    public long Timestamp;
    
    [JsonProperty(PropertyName = "price")]
    public string? Price;

    [JsonProperty(PropertyName = "exchange")]
    public string? Exchange;

    public PricePoint(string exchange = "unknown")
    {
        Exchange = exchange.ToLower();
    }


    public PricePoint(string price, long timestamp, string exchange = "unknown")
    {
        Price = price;
        Timestamp = timestamp;
        Exchange = exchange.ToLower();
    }
}