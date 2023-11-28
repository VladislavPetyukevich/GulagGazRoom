import { FunctionComponent, useCallback, useContext, useEffect, useRef, useState } from 'react';
import { SendMessage } from 'react-use-websocket';
import Peer from 'simple-peer';
import { AuthContext } from '../../../../context/AuthContext';
import { Captions } from '../../../../constants';
import { Transcript } from '../../../../types/transcript';
import { VideoChatVideo } from './VideoChatVideo';
import { VideochatParticipant } from './VideochatParticipant';
import { MessagesChat } from './MessagesChat';
import { getAverageVolume } from './utils/getAverageVolume';
import { createAudioAnalyser, frequencyBinCount } from './utils/createAudioAnalyser';
import { limitLength } from './utils/limitLength';
import { randomId } from './utils/randomId';
import { Devices, EnterVideoChatModal } from './EnterVideoChatModal';
import { Field } from '../../../../components/FieldsBlock/Field';
import { CodeEditor } from '../CodeEditor/CodeEditor';
import { RoomState } from '../../../../types/room';

import './VideoChat.css';

const videoConstraints = {
  height: 300,
  width: 300,
  frameRate: 15,
};

const audioConstraints = {
  channelCount: 1,
  sampleRate: 16000,
  sampleSize: 16,
  volume: 1
};

const audioVolumeThreshold = 10.0;

const transcriptsMaxLength = 100;

const updateLoudedUserTimeout = 5000;

const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;

interface VideoChatProps {
  roomState: RoomState | null;
  roomName?: string;
  viewerMode: boolean;
  lastWsMessage: MessageEvent<any> | null;
  messagesChatEnabled: boolean;
  videoTrackEnabled: boolean;
  audioTrackEnabled: boolean;
  onVideoTrackEnabled: (enabled: boolean) => void;
  onAudioTrackEnabled: (enabled: boolean) => void;
  onSendWsMessage: SendMessage;
};

interface PeerMeta {
  peerID: string;
  nickname: string;
  avatar: string;
  peer: Peer.Instance;
  targetUserId: string;
}

const createTranscript = (body: { userNickname: string; value: string; fromChat: boolean; }): Transcript => ({
  frontendId: randomId(),
  ...body,
});

const enableDisableUserTrack = (stream: MediaStream, kind: string, enabled: boolean) => {
  const track = stream.getTracks().find(track => track.kind === kind);
  if (!track) {
    return;
  }
  track.enabled = enabled;
};

export const VideoChat: FunctionComponent<VideoChatProps> = ({
  roomState,
  roomName,
  viewerMode,
  lastWsMessage,
  messagesChatEnabled,
  audioTrackEnabled,
  videoTrackEnabled,
  onAudioTrackEnabled,
  onVideoTrackEnabled,
  onSendWsMessage,
}) => {
  const auth = useContext(AuthContext);
  const [userStream, setUserStream] = useState<MediaStream | null>(null);
  const [videochatEnabled, setVideochatEnabled] = useState(true);
  const [welcomeScreen, setWelcomeScreen] = useState(true);
  const [transcripts, setTranscripts] = useState<Transcript[]>([createTranscript({
    userNickname: 'GULAG',
    value: `${Captions.ChatWelcomeMessage}, ${auth?.nickname}.`,
    fromChat: true
  })]);
  const userVideo = useRef<HTMLVideoElement>(null);
  const [peers, setPeers] = useState<PeerMeta[]>([]);
  const peersRef = useRef<PeerMeta[]>([]);
  const userIdToAudioAnalyser = useRef<Record<string, AnalyserNode>>({});
  const recognition = useRef(SpeechRecognition ? new SpeechRecognition() : null);
  const [recognitionNotSupported, setRecognitionNotSupported] = useState(false);
  const [recognitionEnabled, setRecognitionEnabled] = useState(false);
  const requestRef = useRef<number>();
  const louderUserId = useRef(auth?.id || '');
  const [videoOrder, setVideoOrder] = useState<Record<string, number>>({
    [auth?.id || '']: 1,
  });
  const updateAnalyserTimeout = useRef(0);
  const intervieweeFrameRef = useRef<HTMLIFrameElement>(null);
  const videochatAvailable = !viewerMode;

  useEffect(() => {
    const frequencyData = new Uint8Array(frequencyBinCount);
    let prevTime = performance.now();
    const updateAudioAnalyser = () => {
      const time = performance.now();
      const delta = time - prevTime;
      if (updateAnalyserTimeout.current > 0) {
        updateAnalyserTimeout.current -= delta;
        prevTime = time;
        requestRef.current = requestAnimationFrame(updateAudioAnalyser);
        return;
      }
      let newLouderUserId = '';
      let louderVolume = -1;
      const result: Record<string, number> = {};
      for (const [userId, analyser] of Object.entries(userIdToAudioAnalyser.current)) {
        analyser.getByteFrequencyData(frequencyData);
        const averageVolume = getAverageVolume(frequencyData);
        if (averageVolume < audioVolumeThreshold) {
          continue;
        }
        if (averageVolume > louderVolume) {
          newLouderUserId = userId;
          louderVolume = averageVolume;
        }
        result[userId] = averageVolume;
      }
      if (newLouderUserId && newLouderUserId !== louderUserId.current) {
        updateAnalyserTimeout.current = updateLoudedUserTimeout;
        setVideoOrder({
          [newLouderUserId]: 1,
          [louderUserId.current]: 2,
        });
        louderUserId.current = newLouderUserId;
      }
      prevTime = time;

      requestRef.current = requestAnimationFrame(updateAudioAnalyser);
    };
    requestRef.current = requestAnimationFrame(updateAudioAnalyser);

    return () => {
      if (requestRef.current) {
        cancelAnimationFrame(requestRef.current);
      }
    };

  }, [louderUserId]);

  const createPeer = useCallback((to: string) => {
    const peer = new Peer({
      initiator: true,
      trickle: false,
      stream: userStream || undefined,
    });

    peer.on('signal', signal => {
      onSendWsMessage(JSON.stringify({
        Type: 'sending signal',
        Payload: JSON.stringify({
          To: to,
          Signal: JSON.stringify(signal),
        }),
      }));
    });

    return peer;
  }, [userStream, onSendWsMessage]);

  const addPeer = useCallback((incomingSignal: Peer.SignalData, callerID: string) => {
    const peer = new Peer({
      initiator: false,
      trickle: false,
      stream: userStream || undefined,
    });

    peer.on('signal', signal => {
      onSendWsMessage(JSON.stringify({
        Type: 'returning signal',
        Payload: JSON.stringify({
          To: callerID,
          Signal: JSON.stringify(signal),
        }),
      }));
    });

    peer.signal(incomingSignal);

    return peer;
  }, [userStream, onSendWsMessage]);

  useEffect(() => {
    const recog = recognition.current;
    return () => {
      if (!userStream) {
        return;
      }
      userStream.getTracks().forEach(track => track.stop());
      recog?.stop();
    };
  }, [userStream]);

  useEffect(() => {
    if (!lastWsMessage?.data || !intervieweeFrameRef.current) {
      return;
    }
    let origin = (new URL(intervieweeFrameRef.current.src)).origin;
    intervieweeFrameRef.current.contentWindow?.postMessage(lastWsMessage.data, origin);
  }, [lastWsMessage]);

  useEffect(() => {
    if (!lastWsMessage || !auth) {
      return;
    }
    try {
      const parsedData = JSON.parse(lastWsMessage?.data);
      if (!parsedData?.Payload) {
        return;
      }
      const parsedPayload = JSON.parse(parsedData?.Payload);
      switch (parsedData?.Type) {
        case 'all users':
          if (!Array.isArray(parsedPayload)) {
            break;
          }
          parsedPayload.forEach(userInChat => {
            if (userInChat.Id === auth.id) {
              return;
            }
            const peer = createPeer(userInChat.Id);
            const newPeerMeta = {
              peerID: userInChat.Id,
              nickname: userInChat.Nickname,
              avatar: userInChat.Avatar,
              targetUserId: userInChat.Id,
              peer,
            };

            peer.on('stream', (stream) => {
              userIdToAudioAnalyser.current[newPeerMeta.targetUserId] = createAudioAnalyser(stream);
            });

            peersRef.current.push(newPeerMeta)
          });
          setPeers([...peersRef.current]);
          break;
        case 'user joined':
          const fromUser = parsedPayload.From;
          const peer = addPeer(JSON.parse(parsedPayload.Signal), fromUser.Id);
          const newPeerMeta = {
            peerID: fromUser.Id,
            nickname: fromUser.Nickname,
            avatar: fromUser.Avatar,
            targetUserId: fromUser.Id,
            peer,
          };
          peersRef.current.push(newPeerMeta);

          peer.on('stream', (stream) => {
            userIdToAudioAnalyser.current[newPeerMeta.targetUserId] = createAudioAnalyser(stream);
          });
          setPeers([...peersRef.current]);
          break;
        case 'user left':
          const leftUserId = parsedPayload.Id;
          const leftUserPeer = peersRef.current.find(p => p.targetUserId === leftUserId);
          if (leftUserPeer) {
            leftUserPeer.peer.destroy();
          }
          const peersAfterLeft = peersRef.current.filter(p => p.targetUserId !== leftUserId);
          peersRef.current = peersAfterLeft;
          setPeers([...peersRef.current]);
          break;
        case 'receiving returned signal':
          const item = peersRef.current.find(p => p.peerID === parsedPayload.From);
          if (item) {
            item.peer.signal(parsedPayload.Signal);
          }
          break;
        default:
          break;
      }
    } catch (err) {
      console.error('parse ws message error: ', err);
    }
  }, [auth, lastWsMessage, addPeer, createPeer]);

  useEffect(() => {
    if (!lastWsMessage) {
      return;
    }
    try {
      const parsedData = JSON.parse(lastWsMessage?.data);
      switch (parsedData?.Type) {
        case 'ChatMessage':
          setTranscripts(transcripts => limitLength(
            [
              ...transcripts,
              createTranscript({
                userNickname: parsedData.Value.Nickname,
                value: parsedData.Value.Message,
                fromChat: true,
              }),
            ],
            transcriptsMaxLength
          ));
          break;
        case 'VoiceRecognition':
          setTranscripts(transcripts => limitLength(
            [
              ...transcripts,
              createTranscript({
                userNickname: parsedData.Value.Nickname,
                value: parsedData.Value.Message,
                fromChat: false,
              }),
            ],
            transcriptsMaxLength
          ));
          break;
        default:
          break;
      }
    } catch (err) {
      console.error('parse chat message error: ', err);
    }
  }, [lastWsMessage]);

  useEffect(() => {
    if (!recognition.current) {
      setRecognitionNotSupported(true);
    }
  }, []);

  useEffect(() => {
    const recog = recognition.current;
    if (!recog) {
      return;
    }
    recog.lang = 'ru';
    recog.continuous = true;
    recog.onend = () => {
      if (recognitionEnabled) {
        recog.start();
      }
    }

    return () => {
      recog.onend = null;
    }
  }, [recognitionEnabled]);

  useEffect(() => {
    const recog = recognition.current;
    if (!recog) {
      return;
    }
    recog.onresult = (event) => {
      for (let i = event.resultIndex; i < event.results.length; i++) {
        const res = event.results[i][0];
        onSendWsMessage(JSON.stringify({
          Type: 'voice-recognition',
          Payload: res.transcript,
        }));
      }
    };

    return () => {
      recog.onresult = null;
    };
  }, [transcripts, auth?.nickname, onSendWsMessage]);

  const handleRecognitionStart = () => {
    if (!recognition.current) {
      return;
    }
    recognition.current.start();
    setRecognitionEnabled(true);
  };

  const handleRecognitionStop = () => {
    if (!recognition.current) {
      return;
    }
    recognition.current.stop();
    setRecognitionEnabled(false);
  };

  const switchAudioRecognition = useCallback((enabled: boolean) => {
    try {
      if (!enabled) {
        handleRecognitionStop();
      } else {
        handleRecognitionStart();
      }
    } catch { }
  }, []);

  const handleVideochatJoin = () => {
    const initUserMedia = async () => {
      try {
        const newUserStream = await navigator.mediaDevices.getUserMedia({
          video: videoConstraints,
          audio: audioConstraints,
        });
        enableDisableUserTrack(newUserStream, 'video', videoTrackEnabled);
        enableDisableUserTrack(newUserStream, 'audio', audioTrackEnabled);
        setUserStream(newUserStream);
        setVideochatEnabled(true);

        onSendWsMessage(JSON.stringify({
          Type: "join video chat",
        }));
      } catch {
        alert(Captions.UserStreamError);
      }
    };
    initUserMedia();
  };

  useEffect(() => {
    if (userVideo.current) {
      userVideo.current.srcObject = userStream;
    }
    if (!userStream || !auth?.id) {
      return;
    }
    try {
      userIdToAudioAnalyser.current[auth.id] = createAudioAnalyser(userStream);
    } catch { }
  }, [userStream, auth?.id]);

  const handleEnableDisableVideo = useCallback((currentUserStream: MediaStream | null, enabled: boolean) => {
    if (currentUserStream) {
      enableDisableUserTrack(currentUserStream, 'video', enabled);
    }
    onVideoTrackEnabled(enabled);
  }, [onVideoTrackEnabled]);

  const handleEnableDisableAudio = useCallback((currentUserStream: MediaStream | null, enabled: boolean) => {
    if (currentUserStream) {
      enableDisableUserTrack(currentUserStream, 'audio', enabled);
    }
    onAudioTrackEnabled(enabled);
  }, [onAudioTrackEnabled]);

  useEffect(() => {
    handleEnableDisableAudio(userStream, audioTrackEnabled);
    if (!welcomeScreen) {
      switchAudioRecognition(audioTrackEnabled);
    }
  }, [userStream, audioTrackEnabled, welcomeScreen, switchAudioRecognition, handleEnableDisableAudio]);

  useEffect(() => {
    handleEnableDisableVideo(userStream, videoTrackEnabled);
  }, [userStream, videoTrackEnabled, handleEnableDisableVideo]);

  const handleTextMessageSubmit = (message: string) => {
    onSendWsMessage(JSON.stringify({
      Type: 'chat-message',
      Payload: message,
    }));
  };

  const handleMediaSelect = useCallback((devices: Devices) => {
    const updateUserStream = async () => {
      const videoRequest = devices.camera?.deviceId && {
        ...videoConstraints,
        deviceId: devices.camera?.deviceId,
      };
      const micRequest = devices.mic?.deviceId && {
        ...audioConstraints,
        deviceId: devices.mic?.deviceId,
      };
      const newUserStream = await navigator.mediaDevices.getUserMedia({
        video: videoRequest || undefined,
        audio: micRequest || undefined,
      });
      setUserStream(newUserStream);
      onAudioTrackEnabled(!!devices.mic?.enabled);
      onVideoTrackEnabled(!!devices.camera?.enabled);
      handleEnableDisableAudio(newUserStream, !!devices.mic?.enabled);
      handleEnableDisableVideo(newUserStream, !!devices.camera?.enabled);
      if (devices.mic?.enabled) {
        handleRecognitionStart();
      } else {
        handleRecognitionStop();
      }
    };
    updateUserStream();
  }, [onAudioTrackEnabled, onVideoTrackEnabled, handleEnableDisableAudio, handleEnableDisableVideo]);

  const handleWelcomeScreenClose = () => {
    setWelcomeScreen(false);
    setVideochatEnabled(true);
  };

  return (
    <div className='room-columns'>
      <EnterVideoChatModal
        open={welcomeScreen}
        roomName={roomName}
        userStream={userStream}
        onClose={handleWelcomeScreenClose}
        onSelect={handleMediaSelect}
      />
      {videochatAvailable && (
        <Field className='videochat-field'>
          {!videochatEnabled && <h3>{Captions.Videochat}</h3>}
          <div className='videochat-switch-buttons'>
          </div>
          {videochatEnabled ? (
            <div className='videochat'>
              <VideochatParticipant
                order={videoOrder[auth?.id || '']}
                avatar={auth?.avatar}
                nickname={auth?.nickname}
                videoTrackEnabled={videoTrackEnabled}
              >
                <video
                  ref={userVideo}
                  className='videochat-video'
                  muted
                  autoPlay
                  playsInline
                >
                  Video not supported
                </video>
              </VideochatParticipant>

              {peers.map(peer => (
                <VideochatParticipant
                  key={peer.targetUserId}
                  order={videoOrder[peer.targetUserId]}
                  avatar={peer?.avatar}
                  nickname={peer?.nickname}
                >
                  <VideoChatVideo peer={peer.peer} />
                </VideochatParticipant>
              ))}
            </div>
          ) : (
            <div>
              <button
                className='videochat-join-button'
                onClick={handleVideochatJoin}
              >
                {Captions.Enter}
              </button>
              <div>{Captions.Warning}</div>
              <div>{Captions.CallRecording}</div>
              {recognitionNotSupported && (<div>{Captions.VoiceRecognitionNotSupported}</div>)}
            </div>
          )}
        </Field>
      )}
      <Field className='videochat-field videochat-field-main'>
        <CodeEditor
          roomState={roomState}
          readOnly={viewerMode}
          lastWsMessage={lastWsMessage}
          onSendWsMessage={onSendWsMessage}
        />
      </Field>
      {!!messagesChatEnabled && (
        <Field className='videochat-field'>
          <MessagesChat
            transcripts={transcripts}
            onMessageSubmit={handleTextMessageSubmit}
          />
        </Field>
      )}
    </div>
  );
};
