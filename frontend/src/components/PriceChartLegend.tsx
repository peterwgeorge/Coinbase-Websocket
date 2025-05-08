import CoinbaseLogo from '../assets/icons/coinbase.svg';
import BinanceLogo from '../assets/icons/binance.png';

export default function PriceChartLegend() {
  return (
    <div style={{ width: 150, paddingLeft: '1rem', display: 'flex', flexDirection: 'column', gap: '1rem' }}>
      <div
        className="text-body"
        style={{
          display: 'flex',
          alignItems: 'center',
          gap: '0.5rem',
          lineHeight: 1,
        }}
      >
        <span style={{ color: '#8884d8', fontSize: '0.75rem' }}>⬤</span>
        <img
          src={CoinbaseLogo}
          alt="Coinbase"
          style={{ height: '1rem', display: 'inline-block' }}
        />
      </div>
      <div
        className="text-body"
        style={{
          display: 'flex',
          alignItems: 'center',
          gap: '0.5rem',
          lineHeight: 1,
        }}
      >
        <span style={{ color: '#f7931a', fontSize: '0.75rem' }}>⬤</span>
        <img
          src={BinanceLogo}
          alt="Binance"
          style={{ height: '1.2rem', display: 'inline-block' }}
        />
      </div>
    </div>
  );
}
