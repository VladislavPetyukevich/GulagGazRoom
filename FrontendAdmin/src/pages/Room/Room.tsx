import { FunctionComponent, useCallback, useContext, useEffect, useState } from 'react';
import { useParams, Navigate } from 'react-router-dom';
import useWebSocket from 'react-use-websocket';
import toast from 'react-hot-toast';
import {
  GetRoomParticipantParams,
  GetRoomQuestionsBody,
  roomQuestionApiDeclaration,
  roomsApiDeclaration,
} from '../../apiDeclarations';
import { MainContentWrapper } from '../../components/MainContentWrapper/MainContentWrapper';
import { REACT_APP_WS_URL } from '../../config';
import { Captions, pathnames } from '../../constants';
import { AuthContext } from '../../context/AuthContext';
import { useApiMethod } from '../../hooks/useApiMethod';
import { useCommunist } from '../../hooks/useCommunist';
import { RoomParticipant, Room as RoomType } from '../../types/room';
import { ActionModal } from '../../components/ActionModal/ActionModal';
import { Reactions } from './components/Reactions/Reactions';
import { ActiveQuestion } from './components/ActiveQuestion/ActiveQuestion';
import { ProcessWrapper } from '../../components/ProcessWrapper/ProcessWrapper';
import { Question } from '../../types/question';
import { VideoChat } from './components/VideoChat/VideoChat';
import { SwitchButton } from './components/VideoChat/SwitchButton';
import { Link } from 'react-router-dom';

import './Room.css';

const getWsStatusMessage = (readyState: number) => {
  switch (readyState) {
    case 0: return 'WS CONNECTING';
    case 2: return 'WS CLOSING';
    case 3: return 'WS CLOSED';
    default: return null;
  }
};

export const Room: FunctionComponent = () => {
  const auth = useContext(AuthContext);
  const { getCommunist } = useCommunist();
  const communist = getCommunist();
  let { id } = useParams();
  const socketUrl = `${REACT_APP_WS_URL}/ws?Authorization=${communist}&roomId=${id}`;
  const { lastMessage, readyState, sendMessage } = useWebSocket(socketUrl);
  const [roomInReview, setRoomInReview] = useState(false);
  const [reactionsVisible, setReactionsVisible] = useState(false);
  const [currentQuestionId, setCurrentQuestionId] = useState<Question['id']>();
  const [currentQuestion, setCurrentQuestion] = useState<Question>();
  const [messagesChatEnabled, setMessagesChatEnabled] = useState(false);
  const [videoTrackEnabled, setVideoTrackEnabled] = useState(false);
  const [audioTrackEnabled, setAudioTrackEnabled] = useState(true);

  const { apiMethodState, fetchData } = useApiMethod<RoomType, RoomType['id']>(roomsApiDeclaration.getById);
  const { process: { loading, error }, data: room } = apiMethodState;

  const {
    apiMethodState: apiRoomStartReviewMethodState,
    fetchData: fetchRoomStartReview,
  } = useApiMethod<unknown, RoomType['id']>(roomsApiDeclaration.startReview);
  const {
    process: { loading: roomStartReviewLoading, error: roomStartReviewError },
  } = apiRoomStartReviewMethodState;

  const {
    apiMethodState: apiOpenRoomQuestions,
    fetchData: getRoomOpenQuestions,
  } = useApiMethod<Array<Question['id']>, GetRoomQuestionsBody>(roomQuestionApiDeclaration.getRoomQuestions);
  const {
    data: openRoomQuestions,
  } = apiOpenRoomQuestions;

  const {
    apiMethodState: apiRoomParticipantState,
    fetchData: getRoomParticipant,
  } = useApiMethod<RoomParticipant, GetRoomParticipantParams>(roomsApiDeclaration.getParticipant);
  const {
    data: roomParticipant,
  } = apiRoomParticipantState;

  const currentUserExpert = roomParticipant?.userType === 'Expert';
  const currentUserExaminee = roomParticipant?.userType === 'Examinee';
  const viewerMode = !(currentUserExpert || currentUserExaminee);

  useEffect(() => {
    if (!auth?.id) {
      return;
    }
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchData(id);
    getRoomOpenQuestions({
      RoomId: id,
      State: 'Active',
    });
    getRoomParticipant({
      RoomId: id,
      UserId: auth.id,
    });
  }, [id, auth?.id, fetchData, getRoomOpenQuestions, getRoomParticipant]);

  useEffect(() => {
    if (!room) {
      return;
    }
    if (room.roomStatus !== 'New') {
      setReactionsVisible(true);
    }
  }, [room]);

  useEffect(() => {
    if (!room || !openRoomQuestions || !openRoomQuestions[0]) {
      return;
    }
    const openQuestion = room.questions.find(roomQ => roomQ.id === openRoomQuestions[0])
    if (!openQuestion) {
      return;
    }
    setCurrentQuestion(openQuestion);
  }, [room, openRoomQuestions]);

  useEffect(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    if (!lastMessage || !auth?.nickname) {
      return;
    }
    try {
      const parsedData = JSON.parse(lastMessage?.data);
      console.log('parsedData: ', parsedData);
      switch (parsedData?.Type) {
        case 'ChatMessage':
          const message = parsedData?.Value?.Message;
          const nickname = parsedData?.Value?.Nickname;
          if (typeof message !== 'string') {
            return;
          }
          if (message.includes(auth.nickname)) {
            toast(`${nickname}: ${message}`, { icon: '💬' });
          }
          break;
        case 'ChangeRoomStatus':
          const newStatus: RoomType['roomStatus'] = 'New';
          const reviewStatus: RoomType['roomStatus'] = 'Review';
          if (parsedData?.Value === reviewStatus) {
            setRoomInReview(true);
          }
          if (parsedData?.Value !== newStatus) {
            setReactionsVisible(true);
          }
          break;
        case 'ChangeRoomQuestionState':
          if (parsedData.Value.NewState !== 'Active') {
            break;
          }
          setCurrentQuestionId(parsedData.Value.QuestionId);
          break;
        default:
          break;
      }
    } catch { }
  }, [id, auth, lastMessage, getRoomOpenQuestions]);

  useEffect(() => {
    if (!currentQuestionId || !room?.questions) {
      return;
    }
    const newCurrentQuestion = room.questions.find(roomQ => roomQ.id === currentQuestionId);
    setCurrentQuestion(newCurrentQuestion);
  }, [currentQuestionId, room?.questions]);

  const handleStartReviewRoom = useCallback(() => {
    if (!id) {
      throw new Error('Room id not found');
    }
    fetchRoomStartReview(id);
  }, [id, fetchRoomStartReview]);

  const handleMessagesChatSwitch = () => {
    setMessagesChatEnabled(!messagesChatEnabled);
  };

  const handleSwitchAudio = () => {
    setAudioTrackEnabled(!audioTrackEnabled);
  };

  const handleSwitchVideo = () => {
    setVideoTrackEnabled(!videoTrackEnabled);
  };

  const loaders = [
    {},
    {},
    { height: '890px' }
  ];

  if (roomInReview && id) {
    return <Navigate to={pathnames.roomAnalyticsSummary.replace(':id', id)} replace />;
  }

  return (
    <MainContentWrapper className="room-wrapper">
      <ProcessWrapper
        loading={loading}
        loadingPrefix={Captions.LoadingRoom}
        loaders={loaders}
        error={error}
        errorPrefix={Captions.ErrorLoadingRoom}
      >
        <>
          <div className="room-page">
            <div className="room-page-main">
              <div className="room-page-header">
                <div>
                  <span
                    className={`room-page-header-caption ${viewerMode ? 'room-page-header-caption-viewer' : ''}`}
                  >
                    <div>{room?.name}</div>
                    {viewerMode && (
                      <div
                        className="room-page-header-question-viewer"
                      >
                        {Captions.ActiveQuestion}: {currentQuestion?.value}
                      </div>
                    )}
                  </span>
                </div>

                {!viewerMode && (
                  <div className="reactions-field">
                    <ActiveQuestion
                      room={room}
                      initialQuestionText={currentQuestion?.value}
                    />
                  </div>
                )}
                {!reactionsVisible && (
                  <div>{Captions.WaitingRoom}</div>
                )}
                {!viewerMode && (
                  <div className='start-room-review'>
                    <ActionModal
                      title={Captions.StartReviewRoomModalTitle}
                      openButtonCaption={Captions.StartReviewRoom}
                      loading={roomStartReviewLoading}
                      loadingCaption={Captions.CloseRoomLoading}
                      error={roomStartReviewError}
                      onAction={handleStartReviewRoom}
                    />
                  </div>
                )}

              </div>
              <div className="room-page-main-content">
                {getWsStatusMessage(readyState) || (
                  <VideoChat
                    roomId={room?.id || ''}
                    roomName={room?.name}
                    viewerMode={viewerMode}
                    lastWsMessage={lastMessage}
                    messagesChatEnabled={messagesChatEnabled}
                    audioTrackEnabled={audioTrackEnabled}
                    videoTrackEnabled={videoTrackEnabled}
                    onAudioTrackEnabled={setAudioTrackEnabled}
                    onVideoTrackEnabled={setVideoTrackEnabled}
                    onSendWsMessage={sendMessage}
                  />
                )}
              </div>
            </div>
            <div className="room-tools-container">
              {!viewerMode && (
                <div className="room-tools room-tools-left">
                  <SwitchButton
                    enabled={audioTrackEnabled}
                    caption={Captions.MicrophoneIcon}
                    subCaption={Captions.Microphone}
                    onClick={handleSwitchAudio}
                  />
                  <SwitchButton
                    enabled={videoTrackEnabled}
                    caption={Captions.CameraIcon}
                    subCaption={Captions.Camera}
                    onClick={handleSwitchVideo}
                  />
                </div>
              )}
              <div className="room-tools room-tools-center">
                <SwitchButton
                  enabled={!messagesChatEnabled}
                  caption={Captions.ChatIcon}
                  subCaption={Captions.Chat}
                  onClick={handleMessagesChatSwitch}
                />
                {reactionsVisible && (
                  <Reactions
                    room={room}
                    roles={auth?.roles || []}
                    participantType={roomParticipant?.userType || null}
                    lastWsMessage={lastMessage}
                  />
                )}
              </div>
              <div className="room-tools room-tools-right">
                <Link to={pathnames.rooms}>
                  <button>{Captions.Exit}</button>
                </Link>
              </div>
            </div>
          </div>
        </>
      </ProcessWrapper>
    </MainContentWrapper>
  );
};
