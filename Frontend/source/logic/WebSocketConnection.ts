interface WebSocketWebSocketConnectionProps {
  url: string;
  communist: string;
  roomId: string;
  onMessage: (ev: MessageEvent<any>) => void;
}

export class WebSocketConnection {
  url: string;
  communist: string;
  roomId: string;
  onMessage: (ev: MessageEvent<any>) => void;

  constructor(props: WebSocketWebSocketConnectionProps) {
    this.url = props.url;
    this.communist = props.communist;
    this.roomId = props.roomId;
    this.onMessage = props.onMessage;
  }

  connect() {
    return new Promise<void>((resolve, reject) => {
      const socketUrl = `${this.url}?Authorization=${this.communist}&roomId=${this.roomId}`;
      const webSocket = new WebSocket(socketUrl);
      webSocket.onmessage = this.onMessage;
      webSocket.onopen = () => resolve();
      webSocket.onclose = () => {
        const message = 'webSocket connection closed';
        alert(message);
        reject(message);
      }
    });
  }
}
