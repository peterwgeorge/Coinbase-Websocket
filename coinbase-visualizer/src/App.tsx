import './App.css';
import { PriceChart } from './components/PriceChart';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <h1>Crypto Exchange Market Data Visualizer</h1>
      </header>
      <main>
        <PriceChart />
      </main>
    </div>
  );
}

export default App;