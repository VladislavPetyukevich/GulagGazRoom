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
  webSocket?: WebSocket;
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
      this.webSocket = new WebSocket(socketUrl);
      this.webSocket.onmessage = this.onMessage;
      this.webSocket.onopen = () => resolve();
      this.webSocket.onclose = () => {
        const message = `webSocket connection closed: ${this.url}`;
        alert(message);
        reject(message);
      }
    });
  }

  send(data: string) {
    if (!this.webSocket) {
      return;
    }
    this.webSocket.send(data);
  }
}
