export interface Binance24hrTickerMessage {
    e: "24hrTicker"; // Event type
    E: number;       // Event time (Unix timestamp in ms)
    s: string;       // Symbol (e.g., BTCUSDT)
  
    p: string;       // Price change (24h)
    P: string;       // Price change percent (24h)
    w: string;       // Weighted average price (24h)
    x: string;       // Previous day's close price
    c: string;       // Current day's close price (last price)
    Q: string;       // Last quantity
  
    b: string;       // Best bid price
    B: string;       // Best bid quantity
    a: string;       // Best ask price
    A: string;       // Best ask quantity
  
    o: string;       // Open price (24h)
    h: string;       // High price (24h)
    l: string;       // Low price (24h)
  
    v: string;       // Total traded base asset volume (24h)
    q: string;       // Total traded quote asset volume (24h)
  
    O: number;       // Statistics open time (ms)
    C: number;       // Statistics close time (ms)
    F: number;       // First trade ID
    L: number;       // Last trade ID
    n: number;       // Total number of trades
  }
  