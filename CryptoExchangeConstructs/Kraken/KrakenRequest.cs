namespace CryptoExchangeConstructs.Kraken;


using Newtonsoft.Json;


public class KrakenRequest
{
    
    [JsonProperty(PropertyName = "method")]
    public string Method;
    [JsonProperty(PropertyName = "params")]
    public KrakenRequestParams Params;

    public KrakenRequest(string method, string[] symbol, string channel)
    {
        Method = method;
        Params = new(symbol, channel);
    }
}