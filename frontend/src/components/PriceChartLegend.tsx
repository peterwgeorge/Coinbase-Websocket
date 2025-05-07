import CoinbaseLogo from '../assets/icons/coinbase.svg';

export default function PriceChartLegend() {
  return (
    <div style={{ width: 150, paddingLeft: '1rem' }}>
      <div
        className="text-body"
        style={{
          display: 'flex',
          alignItems: 'center',
          gap: '0.5rem',
          lineHeight: 1,
        }}
      >
        <span style={{ color: '#8884d8', fontSize: '1.2rem' }}>⬤</span>
        <img
          src={CoinbaseLogo}
          alt="Coinbase"
          style={{ height: '1rem', display: 'inline-block' }}
        />
      </div>
    </div>
  );
}
