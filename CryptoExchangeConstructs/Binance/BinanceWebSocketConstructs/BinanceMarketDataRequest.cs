namespace BinanceWebSocketConstructs;

using Newtonsoft.Json;

public class BinanceMarketDataRequest{
    [JsonProperty(PropertyName = "id")]
    public string Id;

    [JsonProperty(PropertyName = "method")]
    public string Method;

    [JsonProperty(PropertyName = "params")]
    public string[] Params;

    public BinanceMarketDataRequest(string method, string[] symbols){
        Id = Guid.NewGuid().ToString();
        Method = method;
        Params = symbols;
    }
}