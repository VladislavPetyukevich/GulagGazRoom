import ThreeShooter from './view';
import {
  REACT_APP_BACKEND_URL,
  REACT_APP_WS_URL,
} from './logic/env';
import { HTMLElements } from './logic/HTMLElements';
import { Api, RoomState, notAuthenticatedError } from './logic/Api';
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
  onFovUpdate: updateFov,
});

const codeEditor = new CodeEditor(htmlElements.editorContainer);
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
  saveSettings();
}

function updateFov(value: number) {
  if (!threeShooter) {
    return;
  }
  threeShooter.updateFov(value);
  htmlElements.updateFovValue(value);
  saveSettings();
}

const settings = new Settings();

function saveSettings() {
  const settingsData = {
    audioVolume: htmlElements.getAudioVolume(),
    fov: htmlElements.getFov(),
  };
  settings.save(settingsData);
}

let adminsId: string[] = [];

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
const paramsFov = parseInt(urlParams.get('fov') || '');
const paramsMuted = !!urlParams.get('muted');

function checkIsMessageFromToilet(message: string) {
  return message.toLowerCase().startsWith('голос с параши');
}

function createWebSocketMessagehandler(threeShooter: ThreeShooter) {
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
        case 'ReactionLike':
          threeShooter.onPlayerActionStart('like', adminsId.includes(dataParsed.Value.UserId) ? 'admin' : undefined);
          break;
        case 'ReactionDislike':
          threeShooter.onPlayerActionStart('dislike', adminsId.includes(dataParsed.Value.UserId) ? 'admin' : undefined);
          break;
        case 'GasOn':
          threeShooter.onPlayerActionStart('gasEnable');
          break;
        case 'GasOff':
          threeShooter.onPlayerActionStart('gasDisable');
          break;
        case 'EnableCodeEditor':
          codeEditor.show();
          break;
        case 'DisableCodeEditor':
          codeEditor.hide();
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
      htmlElements.setFov(settingsData.fov);
      if (isNaN(paramsFov)) {
        updateFov(settingsData.fov);
      } else {
        threeShooter.updateFov(paramsFov);
      }
    } catch {
      if (!isNaN(paramsFov)) {
        threeShooter.updateFov(paramsFov);
      }
      if (paramsMuted) {
        threeShooter.updateAudioVolume(0);
      }
      console.error('Falied to load game settings');
    }
  }
  loadSetting();
}

function updateCodeEditor(roomState: RoomState) {
  if (roomState.codeEditorContent) {
    codeEditor.setValue(roomState.codeEditorContent);
  }
  if (roomState.enableCodeEditor) {
    codeEditor.show();
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
    const selfAdmin = userSelf.roles.includes('Admin');
    const admins = await api.getAdmins();
    adminsId = admins.map(admin => admin.id);
    const roomState = await api.getRoomState(roomId);
    updateCodeEditor(roomState);
    const rendererSize = htmlElements.getRendererSize();
    const threeShooterProps = {
      renderContainer: htmlElements.renderContainer,
      renderWidth: rendererSize.width,
      renderHeight: rendererSize.height,
      preload: {
        question: roomState.activeQuestion && roomState.activeQuestion.value,
        likes: roomState.likeCount,
        dislikes: roomState.dislikeCount,
      },
      onLoad: onLoad
    };
    threeShooter = new ThreeShooter(threeShooterProps);
    addThreeShooterEventHandlers(threeShooter);
    const webSocketConnection = new WebSocketConnection({
      communist: communistValue,
      roomId,
      url: `${REACT_APP_WS_URL}/ws`,
      onMessage: createWebSocketMessagehandler(threeShooter),
    });
    await webSocketConnection.connect();

    if (selfAdmin) {
      const codeEditorwebSocketConnection = new WebSocketConnection({
        communist: communistValue,
        roomId,
        url: `${REACT_APP_WS_URL}/code`,
        onMessage: (event: MessageEvent<any>) => {
          console.log('message: ', JSON.parse(event.data));
        },
      });
      await codeEditorwebSocketConnection.connect();
      codeEditor.onChange = code => codeEditorwebSocketConnection.send(code);
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
