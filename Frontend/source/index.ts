import ThreeShooter from './view';
import {
  REACT_APP_BACKEND_URL,
  REACT_APP_WS_URL,
} from './logic/env';
import { HTMLElements } from './logic/HTMLElements';
import { Api, RoomState, RoomStateAdditionalStatefulPayload, notAuthenticatedError } from './logic/Api';
import { WebSocketConnection } from './logic/WebSocketConnection';
import { Communist } from './logic/Communist';
import { Settings } from './logic/Settings';
import { CodeEditor } from './logic/CodeEditor';
import { Speech } from './logic/Speech';

let threeShooter: ThreeShooter | null = null;

function onControlsEnabled(enabled: boolean) {
  if (!threeShooter) {
    return;
  }
  threeShooter.setEnabled(enabled);
}

const htmlElements = new HTMLElements({
  onControlsEnabled,
  onAudioVolumeUpdate: updateAudioVolume,
});

let speech: Speech | undefined;
try {
  speech = new Speech();
} catch { }

function updateAudioVolume(value: number) {
  if (!threeShooter) {
    return;
  }
  if (speech) {
    speech.setVolume(value / 10);
  }
  var newVolume = value / 10;
  threeShooter.updateAudioVolume(newVolume);
  htmlElements.updateAudioVolumeValue(value);
  saveSettings();
}

function updateFov(value: number) {
  if (!threeShooter) {
    return;
  }
  threeShooter.updateFov(value);
}

const settings = new Settings();

function saveSettings() {
  const settingsData = {
    audioVolume: htmlElements.getAudioVolume(),
  };
  settings.save(settingsData);
}

const communistValue = new Communist().get();

if (!REACT_APP_BACKEND_URL) {
  throw new Error('REACT_APP_BACKEND_URL are not defined');
}
const api = new Api({
  communist: communistValue,
  url: REACT_APP_BACKEND_URL,
});

const fov = 90;
const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);
const roomId = urlParams.get('roomId');
if (!roomId) {
  alert('Некорректная ссылка на комнату');
  throw new Error('roomId not found in url params');
}
const paramsMuted = !!urlParams.get('muted');

function checkIsMessageFromToilet(message: string) {
  return message.toLowerCase().startsWith('голос с параши');
}

function createWebSocketMessagehandler(threeShooter: ThreeShooter, codeEditor: CodeEditor) {
  return function (event: MessageEvent<any>) {
    try {
      const dataParsed = JSON.parse(event.data);
      switch (dataParsed.Type) {
        case 'ChatMessage':
          const message = `${dataParsed.Value.Nickname}:\n${dataParsed.Value.Message}`;
          if (speech && checkIsMessageFromToilet(String(dataParsed.Value.Message))) {
            speech.speak(dataParsed.Value.Message);
          }
          threeShooter.onPlayerActionStart('chatMessage', message);
          break;
        case 'Like':
          threeShooter.onPlayerActionStart('like', dataParsed.Value.Payload);
          break;
        case 'Dislike':
          threeShooter.onPlayerActionStart('dislike', dataParsed.Value.Payload);
          break;
        case 'Gas':
          threeShooter.onPlayerActionStart('gas');
          break;
        case 'ChangeCodeEditor':
          codeEditor.setValue(dataParsed.Value || '');
          break;
        case 'ChangeRoomQuestionState':
          if (dataParsed.Value.NewState !== 'Active') {
            break;
          }
          const questionId = dataParsed.Value.QuestionId as string;
          api.getQuestion(questionId)
            .then(value => threeShooter.onPlayerActionStart('newQuestion', value))
            .catch(() => alert('Get new question error'));
          break;
        default:
          break;
      }
    } catch {
      console.error('Failed to parse WebSocket message: ', event);
    }
  }
};

function onLoad() {
  console.log('game loaded');
}

function addThreeShooterEventHandlers(threeShooter: ThreeShooter) {
  window.addEventListener('resize', () => {
    const rendererSize = htmlElements.getRendererSize();
    threeShooter.handleResize(
      rendererSize.width,
      rendererSize.height
    );
  });

  function loadSetting() {
    try {
      var settingsData = settings.load();
      htmlElements.setAudioVolume(settingsData.audioVolume);
      if (paramsMuted) {
        threeShooter.updateAudioVolume(0);
      } else {
        updateAudioVolume(settingsData.audioVolume);
      }
    } catch {
      if (paramsMuted) {
        threeShooter.updateAudioVolume(0);
      }
      console.error('Falied to load game settings');
    }
  }
  loadSetting();
}

function updateCodeEditor(codeEditor: CodeEditor, roomState: RoomState) {
  if (roomState.codeEditorContent) {
    codeEditor.setValue(roomState.codeEditorContent);
  }
}

function parseRoomStates(roomState: RoomState) {
  const parsedStates: Record<string, boolean> = {};
  roomState.states.forEach(roomState =>
    parsedStates[roomState.type] = (JSON.parse(roomState.payload) as RoomStateAdditionalStatefulPayload).enabled
  );
  return parsedStates;
};

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
    const parsedStates = parseRoomStates(roomState);
    const rendererSize = htmlElements.getRendererSize();
    const threeShooterProps = {
      renderContainer: htmlElements.renderContainer,
      renderWidth: rendererSize.width,
      renderHeight: rendererSize.height,
      preload: {
        question: roomState.activeQuestion && roomState.activeQuestion.value,
        likes: roomState.likeCount,
        dislikes: roomState.dislikeCount,
        gas: parsedStates.Gas,
      },
      onLoad: onLoad
    };
    threeShooter = new ThreeShooter(threeShooterProps);
    updateFov(fov);
    addThreeShooterEventHandlers(threeShooter);
    const codeEditor = new CodeEditor(htmlElements.editorContainer);
    updateCodeEditor(codeEditor, roomState);
    const webSocketMessagehandler = createWebSocketMessagehandler(threeShooter, codeEditor)
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
