namespace CryptoExchangeConstructs.Kraken;

using CryptoExchangeConstructs.Common;
using Newtonsoft.Json;


public class KrakenRequest : IExchangeRequest
{
    
    [JsonProperty(PropertyName = "method")]
    public string Method;
    [JsonProperty(PropertyName = "params")]
    public KrakenRequestParams Params;

    public KrakenRequest(string method, string channel, string[] symbol)
    {
        Method = method;
        Params = new(symbol, channel);
    }
}