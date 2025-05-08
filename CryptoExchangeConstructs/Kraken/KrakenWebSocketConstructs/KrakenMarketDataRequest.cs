namespace KrakenWebSocketConstructs;

using Newtonsoft.Json;

public class KrakenMarketDataRequest{
    
    [JsonProperty(PropertyName = "method")]
    public string Method;
    [JsonProperty(PropertyName = "params")]
    public KrakenRequestParams Params;

    public KrakenMarketDataRequest(string method, string[] symbol, string channel)
    {
        Method = method;
        Params = new(symbol, channel);
    }


}