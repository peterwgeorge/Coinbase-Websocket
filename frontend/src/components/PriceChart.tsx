// src/components/PriceChart.tsx
import React, { useEffect, useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { webSocketService } from '../services/webSocketService';
import { CoinbaseMessage, TickerEvent } from '../types/coinbase'
import { Binance24hrTickerMessage } from '../types/binance';
import ConnectionStatus from './ConnectionStatus';
import PriceChartLegend from './PriceChartLegend';

interface PricePoint {
  timestamp: number;
  price: number;
}

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
                stroke="#8884d8"
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
        <PriceChartLegend/>
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

  function handleCoinbaseMessage(data: CoinbaseMessage): void {
    const currentProduct = "BTC-USD";
    const event = getTickerEvent(data, currentProduct);
    const price = getPricePoint(data, event, currentProduct);
    updatePriceHistory(price, setCoinbaseData);
  }
  
  function handleBinanceMessage(data: Binance24hrTickerMessage): void {
    const timestamp = data.E;
    const price = parseFloat(data.c);
    updatePriceHistory({ timestamp, price }, setBinanceData);
  }
  
  function isBinanceMessage(data: any): data is Binance24hrTickerMessage {
    return typeof data === 'object' && data !== null && 'e' in data;
  }
  
  function isCoinbaseMessage(data: any): data is CoinbaseMessage {
    return typeof data === 'object' && data !== null && 'channel' in data;
  }

  function onMessage(data: any): void {
    if (isCoinbaseMessage(data)) {
      handleCoinbaseMessage(data);
    } else if (isBinanceMessage(data)) {
      handleBinanceMessage(data);
    } else {
      console.warn("Unknown message source:", data);
    }
  }

  function getTickerEvent(data: CoinbaseMessage, currentProduct: string): TickerEvent | undefined {
    if (data.channel === "ticker" && data.events && data.events.length > 0) {
      const tickerEvent = data.events.find(event =>
        event.type === "update" &&
        event.tickers &&
        event.tickers.some(ticker => ticker.product_id === currentProduct)
      );

      return tickerEvent;
    }
  }

  function getPricePoint(data: CoinbaseMessage, tickerEvent: TickerEvent | undefined, currentProduct: string): PricePoint | undefined {
    if (tickerEvent) {
      const ticker = tickerEvent.tickers.find(t => t.product_id === currentProduct);
      if (ticker) {
        const price = parseFloat(ticker.price);
        const timestamp = new Date(data.timestamp).getTime();

        return { timestamp, price };
      }
    }
  }

  function createAlignedPriceSeries(
    coinbase: PricePoint[],
    binance: PricePoint[]
  ): Array<{ timestamp: number; coinbase?: number; binance?: number }> {
    const timestamps = Array.from(
      new Set([...coinbase.map(p => p.timestamp), ...binance.map(p => p.timestamp)])
    ).sort((a, b) => a - b);
  
    const result = [];
    let lastCoinbase: number | undefined = undefined;
    let lastBinance: number | undefined = undefined;
  
    let cIndex = 0;
    let bIndex = 0;
  
    for (const ts of timestamps) {
      while (cIndex < coinbase.length && coinbase[cIndex].timestamp <= ts) {
        lastCoinbase = coinbase[cIndex].price;
        cIndex++;
      }
  
      while (bIndex < binance.length && binance[bIndex].timestamp <= ts) {
        lastBinance = binance[bIndex].price;
        bIndex++;
      }
  
      result.push({
        timestamp: ts,
        coinbase: lastCoinbase,
        binance: lastBinance
      });
    }
  
    return result;
  }  

  function updatePriceHistory(
    price: PricePoint | undefined,
    setPriceData: React.Dispatch<React.SetStateAction<PricePoint[]>>
  ) {
    if (price && !isNaN(price.price)) {
      const MAX_AGE_MS = 30_000; // Keep last 30 seconds
      const now = Date.now();
  
      setPriceData(prev => {
        // Keep only points within the time window + the new one
        const freshData = prev.filter(p => now - p.timestamp <= MAX_AGE_MS);
        return [...freshData, price];
      });
    }
  }
  
}


