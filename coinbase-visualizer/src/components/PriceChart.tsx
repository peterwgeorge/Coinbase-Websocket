// src/components/PriceChart.tsx
import React, { useEffect, useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { webSocketService } from '../services/webSocketService';

interface PricePoint {
  timestamp: number;
  price: number;
}

interface CoinbaseTicker {
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

interface TickerEvent {
  type: string;
  tickers: CoinbaseTicker[];
}

interface CoinbaseMessage {
  channel: string;
  client_id: string;
  timestamp: string;
  sequence_num: number;
  events: TickerEvent[];
}

export const PriceChart: React.FC = () => {
  const [priceData, setPriceData] = useState<PricePoint[]>([]);
  const [product, setProduct] = useState("BTC-USD");
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    // Connect to WebSocket
    webSocketService.connect();
    setIsConnected(true);

    // Process incoming data
    webSocketService.addDataListener("priceChart", (data: CoinbaseMessage) => {
      // Check if this is a ticker message with events
      if (data.channel === "ticker" && data.events && data.events.length > 0) {
        // Find a ticker update for our selected product
        const tickerEvent = data.events.find(event => 
          event.type === "update" && 
          event.tickers && 
          event.tickers.some(ticker => ticker.product_id === product)
        );
        
        if (tickerEvent) {
          const ticker = tickerEvent.tickers.find(t => t.product_id === product);
          if (ticker) {
            const price = parseFloat(ticker.price);
            const timestamp = new Date(data.timestamp).getTime();
            
            if (!isNaN(price)) {
              setPriceData(prev => {
                // Keep last 100 points for performance
                const newData = [...prev, { timestamp, price }];
                if (newData.length > 100) {
                  return newData.slice(-100);
                }
                return newData;
              });
            }
          }
        }
      }
    });

    return () => {
      webSocketService.removeDataListener("priceChart");
      webSocketService.disconnect();
      setIsConnected(false);
    };
  }, [product]);

  return (
    <div className="price-chart">
      <h2>{product} Price Chart</h2>
      <div>
        <p>Connection Status: {isConnected ? 'Connected' : 'Disconnected'}</p>
        <select value={product} onChange={e => setProduct(e.target.value)}>
          <option value="BTC-USD">BTC-USD</option>
          <option value="ETH-USD">ETH-USD</option>
          <option value="SOL-USD">SOL-USD</option>
        </select>
      </div>
      <div style={{ width: '100%', height: 400 }}>
        <ResponsiveContainer>
          <LineChart data={priceData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis
              dataKey="timestamp"
              domain={['auto', 'auto']}
              tickFormatter={(unixTime) => new Date(unixTime).toLocaleTimeString()}
            />
            <YAxis domain={['auto', 'auto']} />
            <Tooltip
              labelFormatter={(label) => new Date(label).toLocaleString()}
              formatter={(value) => [`$${value}`, 'Price']}
            />
            <Legend />
            <Line
              type="monotone"
              dataKey="price"
              stroke="#8884d8"
              dot={false}
              isAnimationActive={false}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
};