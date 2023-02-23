export type PlayerActionName =
  'gasEnable' |
  'gasDisable';

export interface PlayerAction {
  name: PlayerActionName;
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

  startAction(actionName: PlayerActionName) {
    this.handleAction(actionName);
  }

  handleAction(actionName: PlayerActionName) {
    this.listeners.forEach(listener => {
      if (listener.name === actionName) {
        listener.listener({ name: actionName });
      }
    });
  }
}

const playerActions = new PlayerActions();

export { playerActions };

