import { PricePoint } from "../types/price-point";

export function createAlignedPriceSeries(
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