import './App.css';
import { PriceChart } from './components/PriceChart';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Bitcoin Price Comparison (USD)</h1>
      </header>
      <main>
        <PriceChart />
      </main>
    </div>
  );
}

export default App;