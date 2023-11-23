import {
  REACT_APP_BACKEND_URL,
  REACT_APP_WS_URL,
} from './logic/env';
import { HTMLElements } from './logic/HTMLElements';
import { Api, RoomState, notAuthenticatedError } from './logic/Api';
import { WebSocketConnection } from './logic/WebSocketConnection';
import { Communist } from './logic/Communist';
import { CodeEditor } from './logic/CodeEditor';

const htmlElements = new HTMLElements();


const communistValue = new Communist().get();

if (!REACT_APP_BACKEND_URL) {
  throw new Error('REACT_APP_BACKEND_URL are not defined');
}
const api = new Api({
  communist: communistValue,
  url: REACT_APP_BACKEND_URL,
});

const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);
const roomId = urlParams.get('roomId');
if (!roomId) {
  alert('Некорректная ссылка на комнату');
  throw new Error('roomId not found in url params');
}

function createWebSocketMessagehandler(codeEditor: CodeEditor) {
  return function (event: MessageEvent<any>) {
    try {
      const dataParsed = JSON.parse(event.data);
      switch (dataParsed.Type) {
        case 'ChangeCodeEditor':
          codeEditor.setValue(dataParsed.Value || '');
          break;
        default:
          break;
      }
    } catch {
      console.error('Failed to parse WebSocket message: ', event);
    }
  }
};

function updateCodeEditor(codeEditor: CodeEditor, roomState: RoomState) {
  if (roomState.codeEditorContent) {
    codeEditor.setValue(roomState.codeEditorContent);
  }
}

async function init() {
  if (!REACT_APP_WS_URL) {
    throw new Error('REACT_APP_WS_URL are not defined');
  }
  if (!roomId) {
    alert('Некорректная ссылка на комнату');
    throw new Error('roomId not found in url params');
  }
  try {
    const userSelf = await api.getUserSelf();
    const participant = await api.getParticipant(roomId, userSelf.id);
    const roomState = await api.getRoomState(roomId);
    const codeEditor = new CodeEditor(htmlElements.editorContainer);
    updateCodeEditor(codeEditor, roomState);
    const webSocketMessagehandler = createWebSocketMessagehandler(codeEditor)
    window.addEventListener('message', (event) => {
      webSocketMessagehandler(event);
    });

    const canWriteCode = !!participant && participant.userType !== 'Viewer';
    codeEditor.setReadonly(!canWriteCode);
    if (canWriteCode) {
      const codeEditorwebSocketConnection = new WebSocketConnection({
        communist: communistValue,
        roomId,
        url: `${REACT_APP_WS_URL}/ws`,
        onMessage: (event: MessageEvent<any>) => {
          console.log('message: ', JSON.parse(event.data));
        },
      });
      await codeEditorwebSocketConnection.connect();
      codeEditor.onChange = code => codeEditorwebSocketConnection.send(JSON.stringify({
        Type: 'code',
        Payload: code,
      }));
    }
  } catch (err) {
    if (err === notAuthenticatedError) {
      htmlElements.displayAuthentication();
      return;
    }
    alert(`Init error ${err}`);
    console.error(err);
  }
}
init();
