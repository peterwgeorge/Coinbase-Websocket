// src/components/PriceChart.tsx
import React, { useEffect, useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { webSocketService } from '../services/webSocketService';
import {CoinbaseMessage, TickerEvent, CoinbaseTicker} from '../types/coinbase'

interface PricePoint {
  timestamp: number;
  price: number;
}

export const PriceChart: React.FC = () => {
  const [priceData, setPriceData] = useState<PricePoint[]>([]);
  const [product, setProduct] = useState("BTC-USD");
  const [isConnected, setIsConnected] = useState(false);
    
  useEffect(() => {
    webSocketService.connect();
    setIsConnected(webSocketService.isConnected());
    
    const statusInterval = setInterval(() => {
      setIsConnected(webSocketService.isConnected());
    }, 5000);

    return () => {
      clearInterval(statusInterval);
      webSocketService.disconnect();
      setIsConnected(false);
    };
  }, []);
  useEffect(() => {

    webSocketService.addDataListener("priceChart", (data: CoinbaseMessage) => {
        let tickerEvent = getTickerEvent(data);
        let price = getPricePoint(data, tickerEvent);
        updatePriceHistory(price, setPriceData);
    });

    return () => {
      webSocketService.removeDataListener("priceChart");

    };
  }, [product]);

  return (
    <div className="price-chart">
      <h2>{product} Price Chart</h2>
      <div>
        <p>Connection Status: {isConnected ? 'Connected' : 'Disconnected'}</p>
        <select value={product} onChange={e => setProduct(e.target.value)}>
          <option value="BTC-USD">BTC-USD</option>
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

  function getTickerEvent(data: CoinbaseMessage) : TickerEvent | undefined{
    if (data.channel === "ticker" && data.events && data.events.length > 0) {
      const tickerEvent = data.events.find(event => 
        event.type === "update" && 
        event.tickers && 
        event.tickers.some(ticker => ticker.product_id === product)
      );

      return tickerEvent;
    }
  }

  function getPricePoint(data : CoinbaseMessage, tickerEvent : TickerEvent | undefined) : PricePoint | undefined {
    if (tickerEvent) {
      const ticker = tickerEvent.tickers.find(t => t.product_id === product);
      if (ticker) {
        const price = parseFloat(ticker.price);
        const timestamp = new Date(data.timestamp).getTime();

        return {timestamp, price};
      }
    }
  }

  function updatePriceHistory(price: PricePoint | undefined, setPriceData: React.Dispatch<React.SetStateAction<PricePoint[]>>) {
    if (price) {
      if (!isNaN(price.price)) {
        setPriceData(prev => {
          // Keep last 100 points for performance
          const newData = [...prev, price];
          if (newData.length > 100) {
            return newData.slice(-100);
          }
          return newData;
        });
      }
    }
  }
}


