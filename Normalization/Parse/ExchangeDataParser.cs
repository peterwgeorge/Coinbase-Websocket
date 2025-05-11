namespace Normalization.Parse;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CryptoExchangeModels.Coinbase;
using CryptoExchangeModels.Binance;
using CryptoExchangeModels.Kraken;
using Normalization.Types;


public static class ExchangeDataParser{
    public static bool IsCoinbaseResponse(string json){
        try{
            var jObject = JObject.Parse(json);
            return jObject["channel"] != null && jObject["events"] != null;
        }
        catch{
            return false;
        }
    }

    public static bool IsBinanceResponse(string json){
        try{
            var jObject = JObject.Parse(json);
            return jObject["e"]?.ToString() == "24hrTicker";
        }
        catch{
            return false;
        }
    }

    public static bool IsKrakenResponse(string json){
        try{
            var jObject = JObject.Parse(json);
            return jObject["data"] != null && json.Contains("vwap");
        }
        catch{
            return false;
        }
    }

    public static PricePoint GetPricePoint(string json){
        if(IsCoinbaseResponse(json)){
            CoinbaseResponse? res = JsonConvert.DeserializeObject<CoinbaseResponse>(json);
            CoinbaseTicker? btcTicker = res?.Events?
                .Where(e => e.Type == "update")
                .SelectMany(e => e.Tickers)
                .FirstOrDefault(t => t.ProductId == "BTC-USD");
            return btcTicker != null ? 
               new PricePoint(btcTicker.Price, DateTimeOffset.Parse(res.Timestamp).ToUnixTimeMilliseconds(), "coinbase")
             : new();
        }
        else if(IsBinanceResponse(json)){
            BinanceResponse? res = JsonConvert.DeserializeObject<BinanceResponse>(json);
            return res != null ? new PricePoint(res.LastPrice,res.EventTime, "binance") : new();
        }
        else if(IsKrakenResponse(json)){
            KrakenResponse? res = JsonConvert.DeserializeObject<KrakenResponse>(json);
            return res != null ? new PricePoint(res.Data[0].Last.ToString(), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "kraken") : new();
        }
        else{
            return new PricePoint();
        }
    }
}