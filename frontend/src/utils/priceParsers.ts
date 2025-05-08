import { Binance24hrTickerMessage } from '../types/binance';
import { CoinbaseMessage, TickerEvent } from '../types/coinbase';
import { PricePoint } from '../types/price-point';

export function isBinanceMessage(data: any): data is Binance24hrTickerMessage {
  return typeof data === 'object' && data !== null && 'e' in data;
}

export function isCoinbaseMessage(data: any): data is CoinbaseMessage {
  return typeof data === 'object' && data !== null && 'channel' in data;
}

export function getCoinbaseTickerEvent(data: CoinbaseMessage, currentProduct: string): TickerEvent | undefined {
  if (data.channel === "ticker" && data.events && data.events.length > 0) {
    const tickerEvent = data.events.find(event =>
      event.type === "update" &&
      event.tickers &&
      event.tickers.some(ticker => ticker.product_id === currentProduct)
    );

    return tickerEvent;
  }
}

export function getCoinbasePricePoint(data: CoinbaseMessage, tickerEvent: TickerEvent | undefined, currentProduct: string): PricePoint | undefined {
  if (tickerEvent) {
    const ticker = tickerEvent.tickers.find(t => t.product_id === currentProduct);
    if (ticker) {
      const price = parseFloat(ticker.price);
      const timestamp = new Date(data.timestamp).getTime();

      return { timestamp, price, source: "coinbase" };
    }
  }
}

export function handleCoinbaseMessage(data: CoinbaseMessage): PricePoint | undefined {
  const currentProduct = "BTC-USD";
  const event = getCoinbaseTickerEvent(data, currentProduct);
  const price = getCoinbasePricePoint(data, event, currentProduct);
  return price;
}

export function handleBinanceMessage(data: Binance24hrTickerMessage): PricePoint | undefined {
  const timestamp = data.E;
  const price = parseFloat(data.c);
  return { timestamp, price, source: "binance" };
}

export function extractPricePoint(data: any): PricePoint | undefined {
  if (isCoinbaseMessage(data)) {
    return handleCoinbaseMessage(data);
  }
  else if (isBinanceMessage(data)) {
    return handleBinanceMessage(data);
  }
  else {
    console.warn("Unknown message source:", data);
    return undefined;
  }
}  