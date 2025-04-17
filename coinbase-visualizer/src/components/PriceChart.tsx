// src/components/PriceChart.tsx
import React, { useEffect, useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { webSocketService } from '../services/webSocketService';

interface PricePoint {
  timestamp: number;
  price: number;
}

interface CoinbaseMessage {
  type: string;
  product_id: string;
  changes?: string[][];
  // Add other fields as needed
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
      if (data.product_id === product && data.type === "l2update" && data.changes && data.changes.length > 0) {
        // Extract price from level2 update
        const price = parseFloat(data.changes[0][1]);
        const timestamp = Date.now();
        
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