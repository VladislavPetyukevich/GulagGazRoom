export type RoomStateAdditionalStatefulPayload = {
  enabled: boolean;
};

type RoomStateAdditional = {
  type: string;
  payload: string;
};

export interface RoomState {
  activeQuestion: {
    value: string;
  };
  likeCount: number;
  dislikeCount: number;
  enableCodeEditor: boolean;
  codeEditorContent: string | null;
  states: RoomStateAdditional[];
}

interface User {
  id: string;
  roles: string[];
}

interface Admin {
  id: string;
}

interface Participant {
  id: string;
  roomId: string;
  userId: string;
  userType: 'Viewer' | 'Expert' | 'Examinee';
}

interface ApiProps {
  communist: string;
  url: string;
}

export const notAuthenticatedError = 'Not authenticated';

export class Api {
  communist: string;
  url: string;

  constructor(props: ApiProps) {
    this.communist = props.communist;
    this.url = props.url;
  }

  getUserSelf() {
    return new Promise<User>((resolve, reject) => {
      if (!this.communist) {
        return reject(notAuthenticatedError);
      }
      fetch(`${this.url}/users/self`)
        .then((response) => {
          if (!response.ok) {
            return reject(notAuthenticatedError);
          }
          response.json().then(resolve);
        })
        .catch(() => reject(notAuthenticatedError));
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
      fetch(`${this.url}/users/admins?PageSize=30&PageNumber=1`)
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

  getParticipant(roomId: string, userId: string) {
    return new Promise<Participant | void>((resolve, reject) => {
      fetch(`${this.url}/room-participants?RoomId=${roomId}&UserId=${userId}`)
        .then((response) => {
          if (!response.ok) {
            return reject(new Error('Failed to get participant'));
          }
          response.json().then(json => resolve(json));
        })
    }).catch(() => { });
  }
}
