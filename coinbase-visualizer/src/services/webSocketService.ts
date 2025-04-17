export class WebSocketService {
    private socket: WebSocket | null = null;
    private listeners: Map<string, (data: any) => void> = new Map();
    private reconnectTimeout: NodeJS.Timeout | null = null;
    private url: string;
  
    constructor(url: string) {
      this.url = url;
    }
  
    public connect(): void {
      if (this.socket?.readyState === WebSocket.OPEN) return;
      
      this.socket = new WebSocket(this.url);
      
      this.socket.onopen = () => {
        console.log('WebSocket connected');
        this.clearReconnectTimeout();
      };
      
      this.socket.onmessage = (event) => {
        try {
          const data = JSON.parse(event.data);
          this.listeners.forEach(callback => callback(data));
        } catch (error) {
          console.error('Failed to parse WebSocket message:', error);
        }
      };
      
      this.socket.onclose = () => {
        console.log('WebSocket disconnected, reconnecting...');
        this.scheduleReconnect();
      };
      
      this.socket.onerror = (error) => {
        console.error('WebSocket error:', error);
        this.socket?.close();
      };
    }
    
    public disconnect(): void {
      if (this.socket) {
        this.socket.close();
        this.socket = null;
      }
      
      this.clearReconnectTimeout();
    }
    
    private scheduleReconnect(): void {
      if (!this.reconnectTimeout) {
        this.reconnectTimeout = setTimeout(() => {
          this.connect();
        }, 3000);
      }
    }

    private clearReconnectTimeout(): void {
        if (this.reconnectTimeout) {
          clearTimeout(this.reconnectTimeout);
          this.reconnectTimeout = null;
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
  export const webSocketService = new WebSocketService('ws://localhost:5000/ws');