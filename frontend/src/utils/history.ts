import { PricePoint } from "../types/price-point";

export function retainRecentPrices(
    prev: PricePoint[],
    newPoint: PricePoint,
    maxAgeMs: number
  ): PricePoint[] {
    const now = Date.now();
  
    // Remove old points that are outside the allowed window
    const freshData = prev.filter(p => now - p.timestamp <= maxAgeMs);
  
    // Add the new point and return
    return [...freshData, newPoint];
  }