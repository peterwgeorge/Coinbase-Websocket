export interface CoinbaseTicker {
  type: string;
  product_id: string;
  price: string;
  volume_24_h: string;
  low_24_h: string;
  high_24_h: string;
  low_52_w: string;
  high_52_w: string;
  price_percent_chg_24_h: string;
  best_bid: string;
  best_ask: string;
  best_bid_quantity: string;
  best_ask_quantity: string;
}

export interface TickerEvent {
  type: string;
  tickers: CoinbaseTicker[];
}

export interface CoinbaseMessage {
  channel: string;
  client_id: string;
  timestamp: string;
  sequence_num: number;
  events: TickerEvent[];
}