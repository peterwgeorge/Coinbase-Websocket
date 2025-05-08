namespace KrakenWebSocketConstructs;

using Newtonsoft.Json;

public class KrakenRequestParams{

    [JsonProperty(PropertyName = "channel")]
    public string Channel;
    [JsonProperty(PropertyName = "symbol")]
    public string[] Symbol;
    [JsonProperty(PropertyName = "event_trigger")]
    public string EventTrigger;
    [JsonProperty(PropertyName = "snapshot")]
    public bool Snapshot;

    public KrakenRequestParams(string[] symbol, string channel)
    {
        Channel = channel;
        Symbol = symbol;
        EventTrigger = "trades"; //default value according to https://docs.kraken.com/api/docs/websocket-v2/ticker
        Snapshot = true; //default value according to https://docs.kraken.com/api/docs/websocket-v2/ticker
        //Excluding req_id as it's an optional integer value, and I don't care about tracking my requests at the moment
    }

}