export type PlayerActionName =
  'gas' |
  'like' |
  'dislike' |
  'newQuestion' |
  'chatMessage';

export interface PlayerAction {
  name: PlayerActionName;
  payload: string;
}

export interface PlayerActionListener {
  name: PlayerActionName;
  listener: (action: PlayerAction) => void;
}

class PlayerActions {
  listeners: PlayerActionListener[];
  cameraMovementX: number;

  constructor() {
    this.listeners = [];
    this.cameraMovementX = 0;
  }

  addActionListener(
    actionName: PlayerActionName,
    listener: PlayerActionListener['listener']
  ) {
    this.listeners.push({
      name: actionName,
      listener: listener,
    });
  }

  removeActionListener(
    actionName: PlayerActionName,
    listener: PlayerActionListener['listener']
  ) {
    const listenerIndex = this.listeners.findIndex(
      listenerItem =>
        (listenerItem.name === actionName) &&
        (listenerItem.listener === listener)
    );
    if (listenerIndex === -1) {
      return;
    }
    this.listeners.splice(listenerIndex, 1);
  }

  startAction(actionName: PlayerActionName, payload?: string) {
    this.listeners.forEach(listener => {
      if (listener.name === actionName) {
        listener.listener({ name: actionName, payload: payload || '' });
      }
    });
  }
}

const playerActions = new PlayerActions();

export { playerActions };

