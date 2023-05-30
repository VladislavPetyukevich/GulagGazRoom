interface RoomState {
  activeQuestion: {
    value: string;
  };
  likeCount: number;
  dislikeCount: number;
}

interface Admin {
  id: string;
}

interface ApiProps {
  communist: string;
  url: string;
}

export class Api {
  communist: string;
  url: string;

  constructor(props: ApiProps) {
    this.communist = props.communist;
    this.url = props.url;
  }

  checkAuthorization() {
    return new Promise<void>((resolve, reject) => {
      if (!this.communist) {
        return reject('Not authenticated');
      }
      fetch(`${this.url}/users/self`)
        .then((response) => {
          if (response.ok) {
            return resolve();
          }
          return reject('Not authenticated');
        })
        .catch(() => reject('Not authenticated'));
    });
  }

  getRoomState(roomId: string) {
    return new Promise<RoomState>((resolve, reject) => {
      fetch(`${this.url}/rooms/${roomId}/state`)
        .then((response) => {
          if (!response.ok) {
            return reject(new Error('Failed to get room state'));
          }
          response.json().then(responseJson => {
            if (!this.checkIsRoomStateValid(responseJson)) {
              throw new Error('Received room state is not valid');
            }
            resolve(responseJson);
          });
        });
    });
  }

  checkIsRoomStateValid(roomState: Partial<RoomState>) {
    if (typeof roomState !== 'object') {
      return false;
    }
    if (
      roomState.activeQuestion &&
      typeof roomState.activeQuestion.value !== 'string'
    ) {
      return false;
    }
    if (typeof roomState.likeCount !== 'number') {
      return false;
    }
    if (typeof roomState.dislikeCount !== 'number') {
      return false;
    }
    return true;
  }

  getAdmins() {
    return new Promise<Admin[]>((resolve, reject) => {
      fetch(`${this.url}/users/role/Admin?PageSize=30&PageNumber=1`)
        .then((response) => {
          if (!response.ok) {
            return reject('Cannot get admins');
          }
          response.json().then(responseJson => resolve(responseJson));
        });
    });
  }

  getQuestion(questionId: string) {
    return new Promise<string>((resolve, reject) => {
      fetch(`${this.url}/questions/${questionId}`)
        .then((response) => {
          if (!response.ok) {
            return reject(new Error('Failed to get question'));
          }
          response.json().then(json => resolve(json.value));
        })
    });
  }
}
