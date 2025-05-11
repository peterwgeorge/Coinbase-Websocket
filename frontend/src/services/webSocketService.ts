import { PricePoint } from "../types/price-point";

export class WebSocketService {
    private socket: WebSocket | null = null;
    private listeners: Map<string, (data: any) => void> = new Map();
    private url: string;
  
    constructor(url: string) {
      this.url = url;
    }
  
    public connect(): void {
      if (this.isConnected()) return;
      
      this.socket = new WebSocket(this.url);
      
      this.socket.onopen = () => {
        console.log('WebSocket connected');
      };
      
      this.socket.onmessage = (event) => {
        try {

          if(event.data === "Connected to Coinbase WebSocket relay")
            return;
          
          const data = JSON.parse(event.data) as PricePoint;
          this.listeners.forEach(callback => callback(data));
        } catch (error) {
          console.error('Failed to parse WebSocket message:', error);
        }
      };
      
      this.socket.onclose = (event) => {
        console.log(`WebSocket disconnected. Code: ${event.code}, Reason: ${event.reason}`);
      };
      
      
      this.socket.onerror = (error) => {
        console.error('WebSocket error:', error);
        this.socket?.close();
      };
    }

    public isConnected(): boolean {
      return this.socket?.readyState === WebSocket.OPEN;
    }
    
    public disconnect(): void {
      if (this.socket) {
        this.socket.close();
        this.socket = null;
      }
    }
    
    public addDataListener(id: string, callback: (data: any) => void): void {
      this.listeners.set(id, callback);
    }
    
    public removeDataListener(id: string): void {
      this.listeners.delete(id);
    }
  }
  
  // Create an instance with your local WebSocket URL
  export const webSocketService = new WebSocketService(import.meta.env.VITE_RELAY_SERVER_URL);