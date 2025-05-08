namespace CryptoExchangeConstructs.Binance;

using Newtonsoft.Json;

public class BinanceRequest{
    [JsonProperty(PropertyName = "id")]
    public string Id;

    [JsonProperty(PropertyName = "method")]
    public string Method;

    [JsonProperty(PropertyName = "params")]
    public string[] Params;

    public BinanceRequest(string method, string[] symbols){
        Id = Guid.NewGuid().ToString();
        Method = method;
        Params = symbols;
    }
}