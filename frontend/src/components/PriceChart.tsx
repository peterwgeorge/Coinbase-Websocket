// src/components/PriceChart.tsx
import React, { useEffect, useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { webSocketService } from '../services/webSocketService';
import { createAlignedPriceSeries } from '../utils/dataAlignment';
import { retainRecentPrices } from '../utils/history';
import { extractPricePoint } from '../utils/priceParsers';
import { PricePoint } from '../types/price-point';
import ConnectionStatus from './ConnectionStatus';
import PriceChartLegend from './PriceChartLegend';

export const PriceChart: React.FC = () => {
  const [isConnected, setIsConnected] = useState(false);
  const [coinbaseData, setCoinbaseData] = useState<PricePoint[]>([]);
  const [binanceData, setBinanceData] = useState<PricePoint[]>([]);

  const combinedData = React.useMemo(() => {
    return createAlignedPriceSeries(coinbaseData, binanceData);
  }, [coinbaseData, binanceData]);


  useEffect(() => {
    connectAndListen();
    webSocketService.addDataListener("priceChart", onMessage);
    const intervalId = setInterval(handleStatusInterval, 5000);

    return () => {
      clearInterval(intervalId);
      webSocketService.disconnect();
      setIsConnected(false);
      webSocketService.removeDataListener("priceChart");
    };
  }, []);

  return (
    <div className="price-chart">
      <div>
        <ConnectionStatus isConnected={isConnected} />
      </div>

      <div style={{ display: 'flex', height: 400 }}>
        <div style={{ flex: 1 }}>
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={combinedData}>
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
              <Line
                type="monotone"
                dataKey="coinbase"
                stroke="#0052FF"
                dot={true}
                isAnimationActive={false}
                name="Coinbase"
              />
              <Line
                type="monotone"
                dataKey="binance"
                stroke="#f7931a"
                dot={true}
                isAnimationActive={false}
                name="Binance"
              />
            </LineChart>
          </ResponsiveContainer>
        </div>
        <PriceChartLegend />
      </div>
    </div>

  );

  function connectAndListen() {
    webSocketService.connect();
    setIsConnected(webSocketService.isConnected());
  }

  function handleStatusInterval() {
    if (!webSocketService.isConnected()) {
      console.log("Lost connection. Reconnecting...");
      webSocketService.disconnect();
      connectAndListen();
    }

    setIsConnected(webSocketService.isConnected());
  }

  function onMessage(data: any): void {
    const parsed = extractPricePoint(data);
    if (!parsed)
      return;

    const setter = getHistoryUpdater(parsed.source);
    if (setter) {
      updatePriceHistory(parsed, setter);
    }
  }

  function getHistoryUpdater(source: string): React.Dispatch<React.SetStateAction<PricePoint[]>> | undefined {
    switch (source) {
      case 'coinbase': return setCoinbaseData;
      case 'binance': return setBinanceData;
      default: return undefined;
    }
  }
  
  function updatePriceHistory(
    price: PricePoint | undefined,
    setPriceData: React.Dispatch<React.SetStateAction<PricePoint[]>>
  ) {
    if (price && !isNaN(price.price)) {
      setPriceData(prev => retainRecentPrices(prev, price, 30_000));
    }
  }

}


