import { PricePoint } from "../types/price-point";

export function createAlignedPriceSeries(
  data: Map<string, PricePoint[]>
): Array<{ timestamp: number; [exchange: string]: number | undefined }> {
  // Step 1: Gather all unique timestamps
  const allTimestamps = new Set<number>();
  const sortedData: Map<string, PricePoint[]> = new Map();

  for (const [exchange, points] of data.entries()) {
    const sortedPoints = [...points].sort((a, b) => a.timestamp - b.timestamp);
    sortedData.set(exchange, sortedPoints);
    sortedPoints.forEach(p => allTimestamps.add(p.timestamp));
  }

  const sortedTimestamps = Array.from(allTimestamps).sort((a, b) => a - b);

  // Step 2: Prepare pointers for each exchange
  const indices: Record<string, number> = {};
  const lastPrices: Record<string, number | undefined> = {};
  for (const exchange of data.keys()) {
    indices[exchange] = 0;
    lastPrices[exchange] = undefined;
  }

  // Step 3: Build the aligned series
  const result = [];

  for (const ts of sortedTimestamps) {
    const row: { timestamp: number; [exchange: string]: number | undefined } = { timestamp: ts };

    for (const [exchange, points] of sortedData.entries()) {
      while (
        indices[exchange] < points.length &&
        points[indices[exchange]].timestamp <= ts
      ) {
        lastPrices[exchange] = points[indices[exchange]].price;
        indices[exchange]++;
      }

      row[exchange] = lastPrices[exchange];
    }

    result.push(row);
  }

  return result;
}
