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
import { Field } from '../../../../components/FieldsBlock/Field';
import { CodeEditor } from '../CodeEditor/CodeEditor';
import { RoomState } from '../../../../types/room';
import { parseWsMessage } from './utils/parseWsMessage';
import { UserType } from '../../../../types/user';

import './VideoChat.css';

const audioVolumeThreshold = 10.0;

const transcriptsMaxLength = 100;

const updateLoudedUserTimeout = 5000;

interface VideoChatProps {
  roomState: RoomState | null;
  viewerMode: boolean;
  lastWsMessage: MessageEvent<any> | null;
  messagesChatEnabled: boolean;
  userStream: MediaStream | null;
  videoTrackEnabled: boolean;
  onSendWsMessage: SendMessage;
};

interface PeerMeta {
  peerID: string;
  nickname: string;
  avatar: string;
  peer: Peer.Instance;
  targetUserId: string;
  participantType: UserType;
}

const createTranscript = (body: { userNickname: string; value: string; fromChat: boolean; }): Transcript => ({
  frontendId: randomId(),
  ...body,
});

export const VideoChat: FunctionComponent<VideoChatProps> = ({
  roomState,
  viewerMode,
  lastWsMessage,
  messagesChatEnabled,
  userStream,
  videoTrackEnabled,
  onSendWsMessage,
}) => {
  const auth = useContext(AuthContext);
  const [transcripts, setTranscripts] = useState<Transcript[]>([createTranscript({
    userNickname: 'GULAG',
    value: `${Captions.ChatWelcomeMessage}, ${auth?.nickname}.`,
    fromChat: true
  })]);
  const userVideo = useRef<HTMLVideoElement>(null);
  const [peers, setPeers] = useState<PeerMeta[]>([]);
  const peersRef = useRef<PeerMeta[]>([]);
  const userIdToAudioAnalyser = useRef<Record<string, AnalyserNode>>({});
  const requestRef = useRef<number>();
  const louderUserId = useRef(auth?.id || '');
  const [videoOrder, setVideoOrder] = useState<Record<string, number>>({
    [auth?.id || '']: 1,
  });
  const updateAnalyserTimeout = useRef(0);
  const intervieweeFrameRef = useRef<HTMLIFrameElement>(null);

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
      stream: viewerMode ? undefined : userStream || undefined,
    });

    peer.on('signal', signal => {
      onSendWsMessage(JSON.stringify({
        Type: 'sending signal',
        Value: JSON.stringify({
          To: to,
          Signal: JSON.stringify(signal),
        }),
      }));
    });

    return peer;
  }, [userStream, viewerMode, onSendWsMessage]);

  const addPeer = useCallback((incomingSignal: Peer.SignalData, callerID: string) => {
    const peer = new Peer({
      initiator: false,
      trickle: false,
      stream: viewerMode ? undefined : userStream || undefined,
    });

    peer.on('signal', signal => {
      onSendWsMessage(JSON.stringify({
        Type: 'returning signal',
        Value: JSON.stringify({
          To: callerID,
          Signal: JSON.stringify(signal),
        }),
      }));
    });

    peer.signal(incomingSignal);

    return peer;
  }, [userStream, viewerMode, onSendWsMessage]);

  useEffect(() => {
    return () => {
      if (!userStream) {
        return;
      }
      userStream.getTracks().forEach(track => track.stop());
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
      const parsedMessage = parseWsMessage(lastWsMessage?.data);
      const parsedPayload = parsedMessage?.Value;
      switch (parsedMessage?.Type) {
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
              participantType: userInChat.ParticipantType,
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
            participantType: fromUser.ParticipantType,
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
      const parsedData = parseWsMessage(lastWsMessage?.data);
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
    if (!userStream || !auth?.id) {
      return;
    }
    if (userVideo.current) {
      userVideo.current.srcObject = userStream;
    }
    try {
      userIdToAudioAnalyser.current[auth.id] = createAudioAnalyser(userStream);
    } catch { }
  }, [userStream, auth?.id]);

  const handleTextMessageSubmit = (message: string) => {
    onSendWsMessage(JSON.stringify({
      Type: 'chat-message',
      Value: message,
    }));
  };

  return (
    <div className='room-columns'>
      <Field className='videochat-field'>
        <div className='videochat'>
          <VideochatParticipant
            order={videoOrder[auth?.id || '']}
            viewer={viewerMode}
            avatar={auth?.avatar}
            nickname={`${auth?.nickname} (${Captions.You})`}
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
              viewer={peer.participantType === 'Viewer'}
              order={videoOrder[peer.targetUserId]}
              avatar={peer?.avatar}
              nickname={peer?.nickname}
            >
              <VideoChatVideo peer={peer.peer} />
            </VideochatParticipant>
          ))}
        </div>
      </Field>
      <Field className='videochat-field videochat-field-main'>
        <CodeEditor
          roomState={roomState}
          readOnly={viewerMode}
          lastWsMessage={lastWsMessage}
          onSendWsMessage={onSendWsMessage}
        />
      </Field>
      {!!messagesChatEnabled && (
        <Field className='videochat-field videochat-field-chat'>
          <MessagesChat
            transcripts={transcripts}
            onMessageSubmit={handleTextMessageSubmit}
          />
        </Field>
      )}
    </div>
  );
};
